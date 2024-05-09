using Microsoft.AspNetCore.Components.Forms;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System.Data;

namespace KGPKScheduleParser
{
    public class Parser
    {
        private const string _websiteAdress = "https://schedule.kg-college.ru/";
        private static List<string> _teacherNames = new List<string>();
        private static List<string> separatedTeacherNames = new List<string>();
        /// <summary>
        /// получить список всех преподавателей, чтобы затем отделять их от текста в названии пары
        /// </summary>
        public static void GetAllTeachers()
        {
            ChromeDriver driver = new ChromeDriver();
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



        //НОРМАЛЬНО ПРОРАБОТАТЬ КЕЙС, КОГДА ПАРА НЕ ПО ПОДГРУППАМ. ТЫ АДАПТИРОВАЛ ВЕСЬ ИЗНАЧАЛЬНО НАПИСАННЫЙ КОД ПОД СЛУЧАЙ С ПОДГРУППАМИ.


        public static ScheduleOfWeek GetScheduleForGroup(string groupName)
        {
            ChromeDriver driver = new ChromeDriver();
            driver.Navigate().GoToUrl(_websiteAdress);
            //найти кнопку "группа" и нажать её
            driver.FindElement(By.XPath("/html/body/div[4]/div/button[1]")).Click();
            //найти селект с списком групп
            SelectElement groups = new SelectElement(driver.FindElement(By.XPath("//*[@id=\"selectgroupname\"]")));
            //нажать на искомуюю группу, затем выбрать
            Thread.Sleep(1000);
            groups.SelectByValue(groupName);
            driver.FindElement(By.XPath("//*[@id=\"fGroupCase\"]/div[2]/button[2]")).Click();
            Thread.Sleep(1000);
            //циклом собираю таблички понедельник, вторник.... в лист
            List<IWebElement> allWeekdayBoxes = new();
            for (int i = 1; i <= 5; i++)
            {
                allWeekdayBoxes.Add(driver.FindElement(By.XPath($"/html/body/div[4]/div/div[{i}]")));
            }
            //говнокод начинается здесь
            ScheduleOfWeek schedule = new(); //объедок расписания
            //цикл по каждому боксу, который содержит расписание. весь поиск должен происходить в контексте этого бокса
            //я сейчас буду писать ебаную машину тьюринга для работы со строками
            foreach (var day in allWeekdayBoxes)  //здесь должен создаваться WeekDay
            {
                var dayTitle = day.FindElement(By.XPath("/html/body/div[4]/div/div[1]/div/div[1]/h5")).Text; //Понедельник / 08.04.2024
                var dayName = dayTitle.Split('/')[0];
                var dateOfDay = DateTime.Parse(dayTitle.Split('/')[1]);
                var thisDayRows = day.FindElements(By.TagName("tr"));
                DayOfWeek thisDay = new DayOfWeek();
                foreach (var tableRow in thisDayRows) //здесь должна создаваться Para
                {
                    Thread.Sleep(500);
                    var ClassNumber = tableRow.FindElement(By.TagName("th")); //номер пары лежит в th
                    var shitbag = tableRow.FindElements(By.TagName("td")); //номера кабинетов и общая инфа о паре раскиданы в разных td
                    var comboOfStrings = shitbag[0].Text + " " + shitbag[1].Text; //Сочетание поля с названием пары, преподами итд с полем, в котором написаны кабинеты
                    //надо хотя бы получить кабинеты. По-моему, безопасно взять последние шесть символов из этого сочетания, отпилить их и высосать все числа оттуда
                    if (!String.IsNullOrWhiteSpace(comboOfStrings)) //ЭТО СЛУЧАЕТСЯ, КОГДА ПАРЫ ВООБЩЕ НЕ СУЩЕСТВУЕТ. ОСТАВЛЯЕМ ВСЁ ПУСТЫМ
                    {

                        var stringWithClassroomNumbers = comboOfStrings.Substring(comboOfStrings.Length - 6); //берём последние 6 символов строки
                        var ClassroomNumbersTotal = stringWithClassroomNumbers.Where(char.IsDigit); //получаем из строки только числовые символы. превращаем их в числа, соединяем в строку, затем разбиваем на массив
                        string firstClassNumber = "";
                        string secondClassNumber = "";
                        int[] RoomNumbers = new int[2];

                        if (ClassroomNumbersTotal.Count() == 4)
                        {
                            RoomNumbers[0] = Convert.ToInt32(String.Concat(ClassroomNumbersTotal).Substring(0, 2));
                            RoomNumbers[1] = Convert.ToInt32(String.Concat(ClassroomNumbersTotal).Substring(2, 2));
                        }
                        else
                        {
                            try
                            {
                                RoomNumbers[0] = Convert.ToInt32(String.Concat(ClassroomNumbersTotal));
                            }
                            catch (Exception)
                            {
                                RoomNumbers[0] = 0;
                            }
                        }

                        //теперь надо получить имена преподавателей (преподавателя)
                        //благодаря teacherNames должно быть легко
                        //мне плевать на производительность этой параши
                        Thread.Sleep(500);



                        var TeachersForClass = _teacherNames.FindAll(x => comboOfStrings.ToUpperInvariant().Trim().Contains(x.ToUpperInvariant().Trim())).ToList(); // имеет ли в себе _teacherNames такие элементы x, что строка comboOfStrings в себе их содержит
                        var comboOfStringSplit = comboOfStrings.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)[1].Split();
                        string SurnameToFind = comboOfStringSplit[0].ToUpperInvariant();
                        //var firstTeacherSurname = comboOfStringSplit.FirstOrDefault(x => separatedTeacherNames.Contains(x)); //https://www.youtube.com/watch?v=Nppn3Hnocas
                        var firstTeacherSurname = separatedTeacherNames.First(x => x == SurnameToFind);
                        if (!TeachersForClass[0].Split(' ')[0].Equals(firstTeacherSurname))
                        {
                            TeachersForClass.Reverse();
                        }
                        //теперь надо получить названия пар. Преподаватели были получены в порядке появления, поэтому пары тоже надо получить в порядке появления.
                        //кейс с подгруппами
                        List<string> Classes = new List<string>();
                        if (comboOfStrings.Split(' ').Contains("1п") || comboOfStrings.Split(' ').Contains("2п"))
                        {
                            Classes.Add(String.Concat(comboOfStrings.GetUntilOrEmpty("1п"), " 1п").Trim());
                            Classes.Add(String.Concat(comboOfStrings.ExclusiveGetBetweenOrEmpty("1п", "2п").Trim(), " 2п"));
                        }
                        //если нету разделения на подгруппы, то преподаватель может быть только один.
                        else //String.IsNullOrEmpty(Classes[0]) && String.IsNullOrEmpty(Classes[1])
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
                        schedule.days.Add(thisDay);
                    }
                }
            }
            driver.Quit();
            return schedule;
        }
    }

    public record Para()
    {
        public int classNumber { get; set; }
        public string[] NameOfClasses { get; set; }
        public string[] TeacherNames { get; set; }
        public int[] ClassroomNumbers { get; set; }
    }
    public record DayOfWeek()
    {
        public List<Para> spisokPar { get; set; } = new List<Para>(7);
        public DateOnly date { get; set; }
    }
    public record ScheduleOfWeek()
    {
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

}
