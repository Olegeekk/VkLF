using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using VkNet;
using VkNet.Enums.Filters;
using VkNet.Model;
using VkNet.Model.RequestParams;

namespace VK_NoName
{
    class Program
    {
        static void Main(string[] args)
        {
            //Заводим нашу шарманку =) Отсюдова 
//            Connect();  //просто так
            Thread ConnVk = new Thread(Connect); //Создание объекта потока для соедниения 
            ConnVk.Start(); //Старутем поток и закладываемся на многопоточность 
        }

        private static void Auth()
        {
         //тут планируется сделать единую регистрацию
        }


        private static void Connect()
        {
            Settings settings = Settings.All; // уровень доступа к данным
            //как бы авторизация имееет место быть для того чтобы иметь доступ к определенным функциям API этого вашего контактика
            var api = new VkApi();

            api.Authorize(new ApiAuthParams
            {
                //Указываем данные для авторизации 
                //Для того чтобы получить ApplicationId необходимо создать приложение вконтакте 
                ApplicationId = 0000000,
                Login = "admin@mail.ru",
                Password = "passwordХЗ_WRITE_HERE",
                Settings = Settings.All
            });

            Console.WriteLine("Auth Success\n");

            while (true) // Зацыкливаем основной алгоритм именно тут
            {
                //тут мы обработаем возможность выхода из  приложения при вводе пользователем exit
                Console.WriteLine("Please press Enter or write - exit");
                string line = Console.ReadLine(); // Get string from user
                if (line == "exit") // Check string
                {
                    break;
                }


                //Выводим API токен при успешной авторизации
                //Console.WriteLine("API TOKEN:");
                //Console.WriteLine(api.Token);
                //Запрашиваем у юзверя ввод Айдишника страницЫ
                Console.WriteLine("Enter VK ID :\n");
                try
                {
                    string userId = Console.ReadLine();
                    // Получаем базовую информацию об введенном ID.
                    var profile = api.Users.Get(new string[] { userId }).FirstOrDefault();
                    if (profile == null)
                        return;
                    //Console.WriteLine(p.Id);         // 1
                    Console.WriteLine(profile.FirstName);  //Имя например: "Павел"
                    Console.WriteLine(profile.LastName);   //Фамилия например: "Дуров"

                    //Передаем ID пользователя которого мы планируем читать а также необходимое колличество сообщений
                    var get = api.Wall.Get(new WallGetParams
                    {
                        OwnerId = profile.Id,
                        //колличество меседжей для анализа
                        Count = 5,
                        Extended = true,
                    });
                    string textForMessage = "";

                    //выводим посты со стены пользователя 
                    for (int i = 0; i < get.WallPosts.Count(); i++)
                    {
                      //  Console.WriteLine(i + 1);
                        //Выводим пост с ID 
                        Console.WriteLine(get.WallPosts[i].Text);            //тело записи в дальнейшем можно закоментровать но - админу приложения можно ведь видеть немножко больше публикуемой статистики
                        //Пробуем запостить что-то
                        textForMessage = textForMessage + get.WallPosts[i].Text;




                        //считаем колличество символов в посте, для того чтобы иметь предстваления сколько раз повторять последующий цикл перебора символов при сборе в коллекцию
                        string postForWork = textForMessage.Length.ToString();


                        //делаем "рабочую переменную строку" для дальнейшей обработки
              
                        var res = postForWork
                            .GroupBy(c => c)
                            .ToDictionary(g => g.Key, g => g.Count());
                        Console.WriteLine(res.Count);
                    }
                    //тут мы превратим наше сообщение в ту статистику которую нам нужно видеть на страничке
                    string text = textForMessage.ToString(); //Вводные данные строка
                                                             //  LettersWorker(text);
                    string vinigreat = "@id" + profile.Id + " , статистика для последних 5 постов: {" + LettersWorker(text) + "}" /*  +  "#openstatistic #Wordfrequency #frequency" */; //собираю винигрет из составляющих ингридиентов 
                    
                    VkPublisher(vinigreat); //отправляем запись на публикацию
                    Console.WriteLine(LettersWorker(text));

                }
                catch
                {
                    //отображаем уведомлении об исключении 
                    Console.WriteLine("Возникло Исключеие");
                }
                finally
                {
                    ///   Console.WriteLine("Блок");
                    ///   прост резерв =) закладываемся на масштабируемость


                }
            }
            Console.ReadLine();

        }

        public static string VkPublisher(string vinigreat)
        {
            var api = new VkApi();
            api.Authorize(new ApiAuthParams
            {
                //Указываем данные для авторизации 
                //Для того чтобы получить ApplicationId необходимо создать приложение вконтакте 
                ApplicationId = 0000000,
                Login = "admin@mail.ru",
                Password = "password",
                Settings = Settings.All
            });

            var post = api.Wall.Post(new WallPostParams
            {
                //ух-ты получилось
                Message = vinigreat
                // Message = LettersWorker(text)
            });


            return "Success Publish";
        }



            public static string LettersWorker(string text)
        {

            text = text.ToLower();
            //Console.WriteLine(CountCharacters(text));

            string resultForReturn = "";
            var counter = text.Length.ToString(); //измеряем вводную строку полностью
            double chastotnostContainer; //используем double для того чтобы выхватывать числа с плавающей точкой 
            double Count = 0; //Количество букв
            char PredidChar = '\\'; //Предыдущий символ
            HashSet<string> charactersHush = new HashSet<string>();

            string currentCharacter = "";
            foreach (char characters in text) //Для каждого сиМммвола в строке - опорный символ
            {
                foreach (char cha in text) //Проходим по всем символам
                    if (cha != PredidChar && cha == characters)
                        //И если этот символ не равен предыдущему символу, и одновременно опорному символу
                        Count++; //Увеличиваем количество на 1


                if (Count != 0)
                { //Если количество не равно 0
                  // Console.WriteLine(" символов {0} = {1}", characters, Count); //Выводим опорный символ и количество его в строке
                    try //подстрахуемся на тот случай если вдруг колличество переменных в строке не является численныем 0_о маловероятно но всё-же
                    {


                        var counterInt = Convert.ToInt64(counter); 
                        // собственно тут немножечко ниже мы выситываем частотность
                        chastotnostContainer = Count / counterInt;
                        chastotnostContainer = Math.Round(chastotnostContainer, 5);
                        if (!charactersHush.Contains(Convert.ToString(characters)))
                        {
                            // Console.WriteLine("\"{0}\":{2},", characters, Count, chastotnostContainer); //Выводим опорный символ и количество его в строке
                           resultForReturn = resultForReturn + "\"" + characters + "\":" + chastotnostContainer + ",";
                            

                            //  Console.WriteLine("Количество символов {0} = { 1} частотность {2}", characters, Count, chastotnostContainer); //Выводим опорный символ и количество его в строке
                        }
                    }
                    catch (OverflowException)
                    {
                        Console.WriteLine("{0} Вне Int64 типа.", counter);
                        return "counter";

                    }
                }
                Count = 0; //Обнуляем количество
                PredidChar = characters; //Записываем в предыдущий символ текущий опорный символ

                charactersHush.Add(Convert.ToString(PredidChar));
            }

            //  Console.WriteLine(resultForReturn);
            return resultForReturn; //возвращаем результат для статистики

            //    Console.ReadKey(); //Ожидаем нажатия клавиши

            }



    }
}
