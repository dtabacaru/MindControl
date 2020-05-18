using System;
using System.Collections.Generic;
using System.IO;

namespace RenameAddon
{
    class Program
    {
        static List<string> Adjectives = new List<string>(new string[] { "Able",
                                                                        "Bad",
                                                                        "Best",
                                                                        "Better",
                                                                        "Big",
                                                                        "Black",
                                                                        "Blue",
                                                                        "Certain",
                                                                        "Clear",
                                                                        "Close",
                                                                        "Different",
                                                                        "Dirty",
                                                                        "Early",
                                                                        "Easy",
                                                                        "Economic",
                                                                        "Far",
                                                                        "Federal",
                                                                        "Free",
                                                                        "Full",
                                                                        "Good",
                                                                        "Great",
                                                                        "Green",
                                                                        "Hard",
                                                                        "Holy",
                                                                        "High",
                                                                        "Happy",
                                                                        "Important",
                                                                        "International",
                                                                        "Large",
                                                                        "Late",
                                                                        "Little",
                                                                        "Local",
                                                                        "Long",
                                                                        "Low",
                                                                        "Major",
                                                                        "Military",
                                                                        "National",
                                                                        "New",
                                                                        "Old",
                                                                        "Other",
                                                                        "Political",
                                                                        "Possible",
                                                                        "Public",
                                                                        "Purple",
                                                                        "Real",
                                                                        "Recent",
                                                                        "Red",
                                                                        "Right",
                                                                        "Small",
                                                                        "Social",
                                                                        "Soft",
                                                                        "Special",
                                                                        "Strong",
                                                                        "Sure",
                                                                        "True",
                                                                        "Tiny",
                                                                        "White",
                                                                        "Whole",
                                                                        "Young" });

        static List<string> Verbs = new List<string>(new string[] {"Asking",
                                                                    "Blowing",
                                                                    "Blocking",
                                                                    "Baking",
                                                                    "Breaking",
                                                                    "Blasting",
                                                                    "Calling",
                                                                    "Cleaning",
                                                                    "Crafting",
                                                                    "Cooking",
                                                                    "Coming",
                                                                    "Doing",
                                                                    "Dancing",
                                                                    "Dunking",
                                                                    "Drifting",
                                                                    "Flashing",
                                                                    "Finding",
                                                                    "Flailing",
                                                                    "Farting",
                                                                    "Frying",
                                                                    "Grabbing",
                                                                    "Giving",
                                                                    "Growing",
                                                                    "Going",
                                                                    "Hating",
                                                                    "Hearing",
                                                                    "Helping",
                                                                    "Jousting",
                                                                    "Jumping",
                                                                    "Keeping",
                                                                    "Knowing",
                                                                    "Kicking",
                                                                    "Leaving",
                                                                    "Laughing",
                                                                    "Liking",
                                                                    "Living",
                                                                    "Looking",
                                                                    "Making",
                                                                    "Mining",
                                                                    "Marking",
                                                                    "Moving",
                                                                    "Needing",
                                                                    "Passing",
                                                                    "Playing",
                                                                    "Punching",
                                                                    "Putting",
                                                                    "Running",
                                                                    "Saying",
                                                                    "Seeing",
                                                                    "Seeming",
                                                                    "Smelling",
                                                                    "Slamming",
                                                                    "Stabbing",
                                                                    "Showing",
                                                                    "Starting",
                                                                    "Sliding",
                                                                    "Skating",
                                                                    "Shooting",
                                                                    "Tapping",
                                                                    "Taking",
                                                                    "Talking",
                                                                    "Telling",
                                                                    "Thinking",
                                                                    "Throwing",
                                                                    "Trying",
                                                                    "Turning",
                                                                    "Using",
                                                                    "Walking",
                                                                    "Wanting",
                                                                    "Willing",
                                                                    "Working",});

        static List<string> Nouns = new List<string>(new string[] {"Animal",
                                                                    "Book",
                                                                    "Bed",
                                                                    "Business",
                                                                    "Car",
                                                                    "Child",
                                                                    "Company",
                                                                    "Country",
                                                                    "Day",
                                                                    "Desk",
                                                                    "Dude",
                                                                    "Earth",
                                                                    "Engine",
                                                                    "Flower",
                                                                    "Fighter",
                                                                    "Friend",
                                                                    "Family",
                                                                    "Goon",
                                                                    "Group",
                                                                    "Guy",
                                                                    "Hand",
                                                                    "Hammer",
                                                                    "Heart",
                                                                    "Hotel",
                                                                    "Head",
                                                                    "Home",
                                                                    "Job",
                                                                    "Life",
                                                                    "Lawyer",
                                                                    "Leg",
                                                                    "Lamp",
                                                                    "Man",
                                                                    "Mart",
                                                                    "Month",
                                                                    "Mother",
                                                                    "Miner",
                                                                    "Mug",
                                                                    "Nanny",
                                                                    "Night",
                                                                    "Number",
                                                                    "Office",
                                                                    "Part",
                                                                    "People",
                                                                    "Place",
                                                                    "Point",
                                                                    "Priest",
                                                                    "Player",
                                                                    "Person",
                                                                    "Problem",
                                                                    "Program",
                                                                    "Question",
                                                                    "Ring",
                                                                    "Restaurant",
                                                                    "Room",
                                                                    "Road",
                                                                    "School",
                                                                    "Son",
                                                                    "State",
                                                                    "Story",
                                                                    "Student",
                                                                    "Stud",
                                                                    "System",
                                                                    "Thing",
                                                                    "Task",
                                                                    "Time",
                                                                    "Truck",
                                                                    "Water",
                                                                    "Wind",
                                                                    "Week",
                                                                    "Woman",
                                                                    "Word",
                                                                    "Work",
                                                                    "World",
                                                                    "Year" });

        static Random RandomNumberGenerator = new Random();

        const int SleepTime = 1000 * 60 * 60 * 1;

        //const int SleepTime = 10000;

        static void Main(string[] args)
        {

            try
            {
                while (true)
                {
                    DirectoryInfo mainDir = new DirectoryInfo(Directory.GetCurrentDirectory());

                    List<DirectoryInfo> mainDirDirs = new List<DirectoryInfo>(mainDir.GetDirectories());

                    DirectoryInfo addonDir = null;

                    foreach (DirectoryInfo di in mainDirDirs)
                    {
                        List<FileInfo> directoryFiles = new List<FileInfo>(di.GetFiles());

                        if (directoryFiles.Exists(x => x.Extension.Contains(".toc")))
                        {
                            FileInfo tableOfContents = directoryFiles.Find(x => x.Extension.Contains(".toc"));

                            string tocString = File.ReadAllText(tableOfContents.FullName);

                            if (tocString.Contains("The FYB Team"))
                            {
                                addonDir = di;
                                break;
                            }
                        }

                    }

                    if (addonDir == null)
                        return;

                    List<FileInfo> addonFiles = new List<FileInfo>(addonDir.GetFiles());

                    if (addonFiles.Count != 3)
                        return;

                    string oldAddonName = addonDir.Name;
                    string newAddonName = Adjectives[RandomNumberGenerator.Next(0, 49)] + Verbs[RandomNumberGenerator.Next(0, 49)] + Nouns[RandomNumberGenerator.Next(0, 49)];

                    Directory.CreateDirectory(newAddonName);

                    foreach (FileInfo fi in addonFiles)
                    {
                        if (fi.Extension != ".toc" && fi.Extension != ".lua" && fi.Extension != ".xml")
                            return;

                        string fileString = File.ReadAllText(fi.FullName);

                        fileString = fileString.Replace(oldAddonName, newAddonName);

                        File.WriteAllText(newAddonName + "\\" + newAddonName + fi.Extension, fileString);
                    }

                    Directory.Delete(oldAddonName, true);

                    System.Threading.Thread.Sleep(SleepTime);

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                System.Threading.Thread.Sleep(10 * 1000);
            }
            

        }
    }
}
