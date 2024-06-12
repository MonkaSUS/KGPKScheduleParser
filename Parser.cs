using KGPKScheduleParser;
using Microsoft.AspNetCore.Components.Forms;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System.Collections.ObjectModel;
using System.Data;
using System.Net;
using System.Reflection.Metadata.Ecma335;

namespace KGPKScheduleParser
{
    /// <summary>
    /// ОСТАВЬ НАДЕЖДУ ВСЯК СЮДА ВХОДЯЩИЙ
    ///    ⠀⠀⠀⢀⣀⣤⣤⠴⠶⠶⠶⠶⠶⠶⠶⠶⢤⣤⣀⡀⠀⠀⠀⠀⠀⠀⠀⠀⠀
    ///⠀⠀⠀⠀⢀⣤⠶⠛⠉⠁⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠈⠉⠛⠶⣤⡀⠀⠀⠀⠀⠀
    ///⠀⠀⢀⡴⠛⠁⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠈⠛⢷⡄⠀⠀⠀
    ///⠀⣰⠟⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠹⣦⠀⠀
    ///⢰⠏⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠹⣧⠀
    ///⣿⠀⠀⣤⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢠⡄⠀⢹⡄
    ///⡏⠀⢰⡏⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣿⠀⢸⡇
    ///⣿⠀⠘⣇⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢀⡟⠀⢸⡇
    ///⢹⡆⠀⢹⡆⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣼⠃⠀⣾⠀
    ///⠈⢷⡀⢸⡇⠀⢀⣠⣤⣶⣶⣶⣤⡀⠀⠀⠀⠀⠀⢀⣠⣶⣶⣶⣶⣤⣄⠀⠀⣿⠀⣼⠃⠀
    ///⠀⠈⢷⣼⠃⠀⣿⣿⣿⣿⣿⣿⣿⣿⡄⠀⠀⠀⠀⣾⣿⣿⣿⣿⣿⣿⣿⡇⠀⢸⡾⠃⠀⠀
    ///⠀⠀⠈⣿⠀⠀⢿⣿⣿⣿⣿⣿⣿⣿⠁⠀⠀⠀⠀⢹⣿⣿⣿⣿⣿⣿⣿⠃⠀⢸⡇⠀⠀⠀
    ///⠀⠀⠀⣿⠀⠀⠘⢿⣿⣿⣿⣿⡿⠃⠀⢠⠀⣄⠀⠀⠙⢿⣿⣿⣿⡿⠏⠀⠀⢘⡇⠀⠀⠀
    ///⠀⠀⠀⢻⡄⠀⠀⠀⠈⠉⠉⠀⠀⠀⣴⣿⠀⣿⣷⠀⠀⠀⠀⠉⠁⠀⠀⠀⠀⢸⡇⠀⠀⠀
    ///⠀⠀⠀⠈⠻⣄⡀⠀⠀⠀⠀⠀⠀⢠⣿⣿⠀⣿⣿⣇⠀⠀⠀⠀⠀⠀⠀⢀⣴⠟⠀⠀⠀⠀
    ///⠀⠀⠀⠀⠀⠘⣟⠳⣦⡀⠀⠀⠀⠸⣿⡿⠀⢻⣿⡟⠀⠀⠀⠀⣤⡾⢻⡏⠁⠀⠀⠀⠀⠀
    ///⠀⠀⠀⠀⠀⠀⢻⡄⢻⠻⣆⠀⠀⠀⠈⠀⠀⠀⠈⠀⠀⠀⢀⡾⢻⠁⢸⠁⠀⠀⠀⠀⠀⠀
    ///⠀⠀⠀⠀⠀⠀⢸⡇⠀⡆⢹⠒⡦⢤⠤⡤⢤⢤⡤⣤⠤⡔⡿⢁⡇⠀⡿⠀⠀⠀⠀⠀⠀⠀
    ///⠀⠀⠀⠀⠀⠀⠘⡇⠀⢣⢸⠦⣧⣼⣀⡇⢸⢀⣇⣸⣠⡷⢇⢸⠀⠀⡇⠀⠀⠀⠀⠀⠀⠀
    ///⠀⠀⠀⠀⠀⠀⠀⣷⠀⠈⠺⣄⣇⢸⠉⡏⢹⠉⡏⢹⢀⣧⠾⠋⠀⢠⡇⠀⠀⠀⠀⠀⠀⠀
    ///⠀⠀⠀⠀⠀⠀⠀⠻⣆⠀⠀⠀⠈⠉⠙⠓⠚⠚⠋⠉⠁⠀⠀⠀⢀⡾⠁⠀⠀⠀⠀⠀⠀⠀
    ///⠀⠀⠀⠀⠀⠀⠀⠀⠙⢷⣄⡀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣠⡴⠛⠁⠀⠀⠀⠀⠀⠀⠀⠀
    ///⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠈⠙⠳⠶⠦⣤⣤⣤⡤⠶⠞⠋⠁⠀⠀⠀⠀⠀⠀⠀⠀⠀
    /// </summary>
    public class Parser : IParser
    {
        private const string _websiteAdress = "https://schedule.kg-college.ru/";
        /// <summary>
        /// Список Ф.И.О. преподавателей, берётся из меню выбора "Преподаватели" на сайте расписания.
        /// </summary>
        public static List<string> _teacherNames = new List<string>();
        /// <summary>
        /// Разделённые Ф.И.О. преподавателей.
        /// </summary>
        private static List<string> separatedTeacherNames = new List<string>();
        /// <summary>
        /// получить список всех преподавателей, чтобы затем отделять их от текста в названии пары
        /// </summary>
        public static void GetAllTeachers()
        {
            //ChromeOptions headlessOptions = new ChromeOptions();
            //headlessOptions.AddArguments("headless");
            ChromeDriver driver = new ChromeDriver();//headlessOptions);
            var navigator = driver.Navigate();
            navigator.GoToUrl(_websiteAdress);
            //найти и нажать кнопку Преподаватель
            driver.FindElement(By.XPath("/html/body/div[4]/div/button[2]")).Click();
            Thread.Sleep(500);
            //получить select со всеми именами
            var allTeachersSelect = driver.FindElement(By.XPath("//*[@id=\"selectteachers\"]"));
            SelectElement selectList = new SelectElement(allTeachersSelect);
            _teacherNames.Clear();
            foreach (var opt in selectList.Options)
            {
                _teacherNames.Add(opt.Text.ToUpperInvariant());
            }
            Thread.Sleep(1000);
            foreach (var name in _teacherNames)
            {
                separatedTeacherNames.AddRange(name.ToUpperInvariant().Split(' ')); // "КОГАН" "И.Я."
            }
            driver.Quit();
        }
        public static Groups GetAllGroups()
        {
            List<string> result = new();
            ChromeOptions headlessOptions = new ChromeOptions();
            headlessOptions.AddArguments("headless");
            ChromeDriver driver = new ChromeDriver(headlessOptions);
            driver.Navigate().GoToUrl(_websiteAdress);
            //найти кнопку "группа" и нажать её
            driver.FindElement(By.XPath("/html/body/div[4]/div/button[1]")).Click();
            //найти селект с списком групп
            SelectElement groups = new SelectElement(driver.FindElement(By.XPath("//*[@id=\"selectgroupname\"]")));
            Thread.Sleep(500);
            foreach (var item in groups.Options)
            {
                result.Add(item.Text);
            }
            Groups res = new Groups()
            {
                GroupNames = result,
                dateOfCollection = DateTime.Now
            };
            return res;
        }
        public class Groups
        {
            public List<string> GroupNames { get; set; }
            public DateTime dateOfCollection { get; set; }
        }



        //НОРМАЛЬНО ПРОРАБОТАТЬ КЕЙС, КОГДА ПАРА НЕ ПО ПОДГРУППАМ. ТЫ АДАПТИРОВАЛ ВЕСЬ ИЗНАЧАЛЬНО НАПИСАННЫЙ КОД ПОД СЛУЧАЙ С ПОДГРУППАМИ.


        public ScheduleOfWeek GetScheduleForGroup(string groupName)
        {
            ChromeOptions headlessOptions = new ChromeOptions();
            headlessOptions.AddArguments("headless");
            ChromeDriver driver = new ChromeDriver(headlessOptions);
            driver.Navigate().GoToUrl(_websiteAdress);
            //найти кнопку "группа" и нажать её
            driver.FindElement(By.XPath("/html/body/div[4]/div/button[1]")).Click();
            //найти селект с списком групп
            SelectElement groups = new SelectElement(driver.FindElement(By.XPath("//*[@id=\"selectgroupname\"]")));
            //нажать на искомуюю группу, затем кнопку выбрать
            Thread.Sleep(1000);
            groups.SelectByValue(groupName);
            driver.FindElement(By.XPath("//*[@id=\"fGroupCase\"]/div[2]/button[2]")).Click();
            Thread.Sleep(1000);
            //циклом собираю таблички понедельник, вторник.... в лист
            List<IWebElement> allDaysOfWeek = new();
            for (int i = 1; i <= 5; i++)
            {
                allDaysOfWeek.Add(driver.FindElement(By.XPath($"/html/body/div[4]/div/div[{i}]")));
            }
            //говнокод начинается здесь
            ScheduleOfWeek schedule = new(); //объедок расписания
            //цикл по каждому боксу, который содержит расписание. весь поиск должен происходить в контексте этого бокса
            //я сейчас буду писать ебаную машину тьюринга для работы со строками
            foreach (var day in allDaysOfWeek)  //здесь должен создаваться WeekDay
            {
                var dayTitle = day.FindElement(By.XPath("/html/body/div[4]/div/div[1]/div/div[1]/h5")).Text; //Находим строку Понедельник / 08.04.2024
                var dayName = dayTitle.Split('/')[0]; //Отделяем название дня недели
                var dateOfDay = DateTime.Parse(dayTitle.Split('/')[1]); //Отделяем дату
                var thisDayRows = day.FindElements(By.TagName("tr")); //Получаем все строки данного дня
                DayOfWeek thisDay = new DayOfWeek(); //объедок дня недели
                foreach (var tableRow in thisDayRows) //здесь должна создаваться Para
                {
                    Thread.Sleep(500); //ждёмс
                    var shitbag = tableRow.FindElements(By.TagName("td")); //номера кабинетов и общая инфа о паре раскиданы в разных td. Комбинируем их в одну строку
                    var ClassNumber = tableRow.FindElement(By.TagName("th")); //номер пары лежит в th. получаем его отдельно.
                    var comboOfStrings = shitbag[0].Text + " " + shitbag[1].Text; //Создаём сочетание поля с названием пары, преподами итд с полем, в котором написаны кабинеты
                    //надо хотя бы получить кабинеты. По-моему, безопасно взять последние шесть символов из этого сочетания, отпилить их и высосать все числа оттуда
                    if (!String.IsNullOrWhiteSpace(comboOfStrings)) //ЭТО СЛУЧАЕТСЯ, КОГДА ПАРЫ ВООБЩЕ НЕ СУЩЕСТВУЕТ. НАПРИМЕР, ВОСЬМАЯ ПАРА??. ОСТАВЛЯЕМ ВСЁ ПУСТЫМ
                    {

                        var stringWithClassroomNumbers = comboOfStrings.Substring(comboOfStrings.Length - 6); //берём последние 6 символов общей строки и сохраняем.
                        var ClassroomNumbersTotal = stringWithClassroomNumbers.Where(char.IsDigit); //получаем из строки только числовые символы. превращаем их в числа, соединяем в строку, затем разбиваем на массив. ЕСЛИ ТАМ c/з, то кабинет будет равен 0
                        string firstClassNumber = ""; //первый кабинет
                        string secondClassNumber = ""; //второй кабинет
                        int[] RoomNumbers = new int[2];

                        if (ClassroomNumbersTotal.Count() == 4) //если мы в сумме имеем 4 символа в номерах кабинетов, то пары две, разделяем их пополам.
                        {
                            RoomNumbers[0] = Convert.ToInt32(String.Concat(ClassroomNumbersTotal).Substring(0, 2));
                            RoomNumbers[1] = Convert.ToInt32(String.Concat(ClassroomNumbersTotal).Substring(2, 2));
                        }
                        else //значит пара одна
                        {
                            try
                            {
                                var concatedStrings = String.Concat(ClassroomNumbersTotal);
                                if (string.IsNullOrWhiteSpace(concatedStrings) || concatedStrings == "1" || concatedStrings == "2")
                                {
                                    RoomNumbers[0] = 0;
                                }
                                else
                                {
                                    RoomNumbers[0] = Convert.ToInt32(concatedStrings); // пытаемся получить номер кабинета
                                }
                            }
                            catch (Exception) //исключение выплёвывается, если не получилось спарсить ни одного числа. ставим кабинет = 0
                            {
                                RoomNumbers[0] = 0;
                            }
                        }

                        //теперь надо получить имена преподавателей (преподавателя)
                        //благодаря teacherNames должно быть легко
                        //мне плевать на производительность этой параши
                        Thread.Sleep(500);


                        ///
                        /// Здесь случилась самая страшная проблема
                        /// Иногда, когда пара разбита по подруппам, преподаватели пар идут в алфавитном порядке, а иногда нет.
                        /// В том виде, в котором мы получаем всех преподавателей, они всегда в алфавитном порядке
                        /// То есть, если в расписании стоит Ин.Яз., ведут его
                        /// Крюкова 1п.
                        /// Карпенко 2п.
                        /// А мы в поле имеем Карпенко 1п., Крюкова 2п.
                        /// Ниже идёт код, который избавляется от этой проблемы
                        ///


                        var TeachersForClass = _teacherNames.FindAll(x => comboOfStrings.ToUpperInvariant().Trim().Contains(x.ToUpperInvariant().Trim())).ToList(); // имеет ли в себе _teacherNames такие элементы x, что строка comboOfStrings в себе их содержит
                        if (!string.IsNullOrEmpty(string.Join("", TeachersForClass)))
                        {

                            var comboOfStringSplit = comboOfStrings.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)[1].Split(); //разделяем общую строку по лайнбрейку
                            string SurnameToFind = comboOfStringSplit[0].ToUpperInvariant(); //Фамилия, которую надо найти
                            //https://www.youtube.com/watch?v=Nppn3Hnocas
                            var firstTeacherSurname = separatedTeacherNames.First(x => x == SurnameToFind); //получаем фамилию первого реального преподавателя
                            if (!TeachersForClass[0].Split(' ')[0].Equals(firstTeacherSurname)) //если первая реальная фамилия не сочетается с той, что мы получили в начале, меняем их местами
                            {
                                TeachersForClass.Reverse();
                            }
                        }
                        else
                        {
                            TeachersForClass = thisDay.spisokPar.First(x => !string.IsNullOrEmpty(x.NameOfClasses[0])).TeacherNames.ToList();
                        }

                        //теперь надо получить названия пар. Преподаватели были получены в порядке появления, поэтому пары тоже надо получить в порядке появления.
                        //кейс с подгруппами
                        bool IsSubgroups = false;

                        List<string> Classes = new List<string>();
                        if (comboOfStrings.Split(' ').Contains("1п"))
                        {
                            Classes.Add(String.Concat(comboOfStrings.GetUntilOrEmpty("1п"), " 1п").Trim());
                            IsSubgroups = true;
                        }
                        if (comboOfStrings.Split(' ').Contains("2п"))
                        {
                            Classes.Add(String.Concat(comboOfStrings.ExclusiveGetBetweenOrEmpty("1п", "2п").Trim(), " 2п"));
                            IsSubgroups = true;
                        }
                        //если нету разделения на подгруппы, то преподаватель может быть только один.
                        if (!IsSubgroups) //String.IsNullOrEmpty(Classes[0]) && String.IsNullOrEmpty(Classes[1])
                        {
                            string nameOfClassToAdd = comboOfStrings.Split(new char[] { '\r', '\n' })[0];
                            Classes.Add(nameOfClassToAdd);
                        }
                        Para thisClass = new Para()
                        {
                            classNumber = Convert.ToInt32(ClassNumber.Text),
                            ClassroomNumbers = RoomNumbers,
                            NameOfClasses = Classes.ToArray(),
                            TeacherNames = TeachersForClass.ToArray()
                        };
                        thisDay.spisokPar.Add(thisClass);
                        thisDay.date = DateOnly.FromDateTime(dateOfDay);
                    }
                }
                schedule.days.Add(thisDay);
            }
            driver.Quit();
            return schedule;
        }
    }
    /// <summary>
    /// Базовое представление пары
    /// </summary>
    public record Para()
    {
        /// <summary>
        /// Порядковый номер пары.
        /// </summary>
        public int classNumber { get; set; }
        /// <summary>
        /// Название всех пар. Пара может быть одна или две.
        /// </summary>
        public string[] NameOfClasses { get; set; }
        /// <summary>
        /// Ф.И.О. Преподавателей. Может быть одно или два
        /// </summary>
        public string[] TeacherNames { get; set; }
        /// <summary>
        /// Номера кабинетов. Может быть один или два
        /// </summary>
        public int[] ClassroomNumbers { get; set; }
    }
    /// <summary>
    /// Базовое представление дня недели
    /// </summary>
    public record DayOfWeek()
    {
        /// <summary>
        /// Пары, которые содержатся в этом дне.
        /// </summary>
        public List<Para> spisokPar { get; set; } = new List<Para>(7);
        /// <summary>
        /// Дата. (мне очень хочется написать день дня)
        /// </summary>
        public DateOnly date { get; set; }
    }
    /// <summary>
    /// Само расписание
    /// </summary>
    public record ScheduleOfWeek()
    {
        /// <summary>
        /// Дни недели с понедельника по пятницу
        /// </summary>
        public List<DayOfWeek> days { get; set; } = new List<DayOfWeek>(5);
    }
    static class Helper
    {
        /// <summary>
        /// НАГЛО УКРАДЕНО С СТАКОВЕРФЛОУ
        /// </summary>
        /// <param name="text"></param>
        /// <param name="stopAt">текст, до которого ведётся поиск</param>
        /// <returns></returns>
        public static string GetUntilOrEmpty(this string text, string stopAt)
        {
            if (!String.IsNullOrWhiteSpace(text))
            {
                int charLocation = text.IndexOf(stopAt, StringComparison.Ordinal);

                if (charLocation > 0 && charLocation != -1)
                {
                    return text.Substring(0, charLocation);
                }
            }

            return String.Empty;
        }
        public static string ExclusiveGetBetweenOrEmpty(this string text, string startAt, string stopAt)
        {
            if (!String.IsNullOrWhiteSpace(text))
            {
                int startLocation = text.IndexOf(startAt, StringComparison.Ordinal);
                int stopLocation = text.IndexOf(stopAt, StringComparison.Ordinal);

                if (startLocation == -1)
                {
                    return text.GetUntilOrEmpty(stopAt).Split(new char[] { '\r', '\n' })[0];
                }
                else
                {

                    if (stopLocation > startLocation && stopLocation is > 0 and not -1 && startLocation is > 0 and not -1)
                    {
                        return text.Substring(startLocation + startAt.Length, stopLocation - startLocation - startAt.Length);
                    }
                }
            }
            return String.Empty;
        }
    }
    public interface IParser
    {
        public static void GetAllTeachers()
        {

        }
        public ScheduleOfWeek GetScheduleForGroup(string groupName)
        {
            return new ScheduleOfWeek();
        }
    }

}


