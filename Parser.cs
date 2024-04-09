using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System.Data;

namespace KGPKScheduleParser
{
    public static class Parser
    {
        private const string _websiteAdress = "https://schedule.kg-college.ru/";
        private static List<string> _teacherNames = new List<string>();
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
            //получить select со всеми именами
            var allTeachersSelect = driver.FindElement(By.XPath("//*[@id=\"selectteachers\"]"));
            SelectElement selectList = new SelectElement(allTeachersSelect);
            _teacherNames.Clear();
            foreach (var opt in selectList.Options)
            {
                _teacherNames.Add(opt.Text);
            }
        }
        public static ScheduleOfWeek GetScheduleForGroup(string groupName)
        {
            ChromeDriver driver = new ChromeDriver();
            driver.Navigate().GoToUrl(_websiteAdress);
            //найти кнопку "группа" и нажать её
            driver.FindElement(By.XPath("/html/body/div[4]/div/button[1]")).Click();
            //найти селект с списком групп
            SelectElement groups = new SelectElement(driver.FindElement(By.XPath("//*[@id=\"selectgroupname\"]")));
            //нажать на искомуюю группу, затем выбрать
            groups.SelectByValue(groupName);
            driver.FindElement(By.XPath("//*[@id=\"fGroupCase\"]/div[2]/button[2]")).Click();
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
                var dateOfDay = DateTime.Parse(dayTitle.Split('/')[1]).ToShortDateString();
                var thisDayRows = day.FindElements(By.TagName("tr"));
                foreach (var tableRow in thisDayRows) //здесь должна создаваться Para
                {
                    var ClassNumber = tableRow.FindElement(By.TagName("th")); //номер пары лежит в th
                    var shitbag = tableRow.FindElements(By.TagName("td")); //номера кабинетов и общая инфа о паре раскиданы в разных td
                    var comboOfStrings = shitbag[0].Text + " " + shitbag[1].Text; //Сочетание поля с названием пары, преподами итд с полем, в котором написаны кабинеты
                    //надо хотя бы получить кабинеты. По-моему, безопасно взять последние шесть символов из этого сочетания, отпилить их и высосать все числа оттуда
                    var stringWithClassroomNumbers = comboOfStrings.Substring(comboOfStrings.Length - 6); //берём последние 6 символов строки
                    var actualClassroomNumbers = String.Join(" ", stringWithClassroomNumbers.Where(char.IsDigit)).Split(' '); //получаем из строки только числовые символы. превращаем их в числа, соединяем в строку, затем разбиваем на массив


                    int[] RoomNumbers = new int[2];
                    foreach (var item in actualClassroomNumbers)
                    {
                        RoomNumbers[0] = Convert.ToInt32(item);
                    }
                    //теперь надо получить имена преподавателей (преподавателя)
                    //благодаря teacherNames должно быть легко
                    //мне плевать на производительность этой параши
                    var TeachersForClass = _teacherNames.FindAll(x => comboOfStrings.Contains(x)); // имеет ли в себе _teacherNames такие элементы x, что строка comboOfStrings в себе их содержит
                    //теперь надо получить названия пар. Преподаватели были получены в порядке появления, поэтому пары тоже надо получить в порядке появления.
                    //кейс с подгруппами
                    var firstClass = comboOfStrings.GetUntilOrEmpty("1п");
                    var secondClass = comboOfStrings.GetUntilOrEmpty("2п");
                }
            }
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

                if (charLocation > 0 && charLocation != 1)
                {
                    return text.Substring(0, charLocation);
                }
            }

            return String.Empty;
        }
    }

}
