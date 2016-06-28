using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.Content;
using Android.Content.PM;
using Android.Database;
using Android.Provider;
using Xamarin.Forms;
using Uri = Android.Net.Uri;

namespace XamarinForm.Droid
{
    public class CollectEngine
    {
        private StackLayout _stackLayout;

        public CollectEngine()
        {
            _stackLayout = new StackLayout()
            {
                Orientation = StackOrientation.Vertical
            };

        }

        public void CollectApplications(PackageManager packageManager)
        {
            AddTitle("Applications");
            var apps = packageManager.GetInstalledApplications(PackageInfoFlags.MatchAll);
            AddTotalCount(apps.Count);

        }
        public void CollectCalendarEvents(ContentResolver contentResolver)
        {
            AddTitle("Calendar events");
            var calendarUri = CalendarContract.Events.ContentUri;
            var posixTime = DateTime.SpecifyKind(new DateTime(1970, 1, 1), DateTimeKind.Utc);
            string[] eventsProjection = {
                            CalendarContract.Events.InterfaceConsts.Title,
                            CalendarContract.Events.InterfaceConsts.Dtstart

            };
            ICursor cur = contentResolver.Query(calendarUri, eventsProjection, null, null, null);
            cur.MoveToFirst();

            var events = new List<string>();
            while (cur.MoveToNext())
            {
                string title = cur.GetString(cur.GetColumnIndexOrThrow(CalendarContract.Events.InterfaceConsts.Title));
                string date = cur.GetString(cur.GetColumnIndexOrThrow(CalendarContract.Events.InterfaceConsts.Dtstart));
                events.Add("Title: " + title + " .Date: " + posixTime.AddMilliseconds(double.Parse(date)).ToShortDateString());
            }
            AddTotalCount(events.Count);
        }

        public void CallHistory(ContentResolver contentResolver)
        {
            AddTitle("Calls history");

            var callUri = CallLog.Calls.ContentUri;
            var posixTime = DateTime.SpecifyKind(new DateTime(1970, 1, 1), DateTimeKind.Utc);

            string[] eventsProjection = {
               CallLog.Calls.Date,
               CallLog.Calls.Type,
               //type2 outcoming,type1 incoming,type3 missing
            };

            ICursor cur = contentResolver.Query(callUri, eventsProjection, null, null, null);
            var events = new List<string>();
            cur.MoveToFirst();

            while (cur.MoveToNext())
            {
                string date = cur.GetString(cur.GetColumnIndex(CallLog.Calls.Date));
                string type = cur.GetString(cur.GetColumnIndexOrThrow(CallLog.Calls.Type));
                string call = string.Empty;
                switch (type)
                {
                    case "type1":
                        call = "incoming"; break;
                    case "type2":
                        call = "outcoming";
                        break;
                    case "type3":
                        call = "missing";
                        break;
                }
                events.Add(type);

            }

            AddTotalCount($"Total incoming calls - {events.Count(typeCall => typeCall == 1.ToString())}");
            //AddTotalCount($"Total other - {events.Count(typeCall => Enumerable.Range(1,3).Any(n=>n.ToString()!=typeCall))}");
            AddTotalCount($"Total outcoming calls - {events.Count(typeCall => typeCall == 2.ToString())}");
            AddTotalCount($"Total missing calls - {events.Count(typeCall => typeCall == 3.ToString())}");
        }
        public void GetAllSms(ContentResolver contentResolver)
        {
            AddTitle("Sms");

            var sms = new List<string>();
            ICursor cur = contentResolver.Query(Telephony.Sms.ContentUri, null, null, null, null);
            cur.MoveToFirst();
            while (cur.MoveToNext())
            {
                string address = cur.GetString(cur.GetColumnIndex("address"));
                sms.Add("Sms phone number: " + address);

            }
            AddTotalCount(sms.Count);
        }
        public void GetBrowserHistory(ContentResolver contentResolver)
        {
            AddTitle("Browser google chrome");

            string[] lProject = new string[] { Browser.BookmarkColumns.Date,
            Browser.BookmarkColumns.Title, Browser.BookmarkColumns.Url, Browser.BookmarkColumns.Visits };
            string lSelect = Browser.BookmarkColumns.Bookmark + " = 0";
            Uri uriCustom = Uri.Parse("content://com.android.chrome.browser/bookmarks");
            var lItem = contentResolver.Query(uriCustom, lProject, lSelect, null, null);
            if (lItem == null)
            {
                AddTotalCount("Browser google chrome is not intalled");
                return;
            }

            lItem.MoveToFirst();

            string title = string.Empty;
            string url = string.Empty;
            var listBookmarks = new List<Link>();
            if (lItem.MoveToFirst() && lItem.Count > 0)
            {
                bool lContinue = true;
                while (lItem.IsAfterLast == false && lContinue)
                {
                    title = lItem.GetString(lItem.GetColumnIndex(Browser.BookmarkColumns.Title));
                    var date = lItem.GetString(lItem.GetColumnIndex(Browser.BookmarkColumns.Date));
                    url = lItem.GetString(lItem.GetColumnIndex(Browser.BookmarkColumns.Url));
                    listBookmarks.Add(new Link()
                    {
                        Date = date,
                        Url = url
                    });
                    lItem.MoveToNext();
                }
                AddTotalCount(listBookmarks.Count);
            }
        }
        public void OtherAddressBook(ContentResolver contentResolver)
        {
            AddTitle("Address book");

            Uri uri = ContactsContract.Contacts.ContentUri;//CommonDataKinds.Phone.ContentUri;
            string[] projection = new string[] {
            //ContactsContract.CommonDataKinds.Phone.SearchDisplayNameKey,
                ContactsContract.Contacts.InterfaceConsts.Count};

            ICursor people = contentResolver.Query(uri, projection, null, null, null);
            people.MoveToFirst();

            //int indexName = people.GetColumnIndex(projection[0]);
            string count = people.GetString(people.GetColumnIndex(ContactsContract.Contacts.InterfaceConsts.Count));

            AddTotalCount(int.Parse(count));

            string result = string.Empty;
            //do
            //{
            //    //string name = people.GetString(indexName);
            //    string number = people.GetString(indexNumber);
            //    result += number;

            //    // Do work...
            //} while (people.MoveToNext());
            //_collectData.Add(result);
        }

        public StackLayout Get()
        {
            //return _collectData.Aggregate(new StringBuilder(), (sb, s) => sb.AppendLine(s)).ToString();
            return _stackLayout;
        }

        public string GetString()
        {
            return string.Join(Environment.NewLine, _stackLayout.Children.Select(label => ((Label)label).Text));
        }

        public void ReBuild()
        {
            _stackLayout = new StackLayout()
            {
                Orientation = StackOrientation.Vertical
            };
        }

        private void AddTitle(string text)
        {
            var label = new Label()
            {
                Text = text.ToUpper(),
                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
                HorizontalTextAlignment = TextAlignment.Center,
                TextColor = Color.Red
            };

            _stackLayout.Children.Add(label);
        }
        private void AddTotalCount(int count)
        {
            var label = new Label()
            {
                Text = $"Total - {count}",
                FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
                HorizontalTextAlignment = TextAlignment.Start
            };

            _stackLayout.Children.Add(label);
        }
        private void AddTotalCount(string text)
        {
            var label = new Label()
            {
                Text = text,
                FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
                HorizontalTextAlignment = TextAlignment.Start
            };

            _stackLayout.Children.Add(label);
        }
    }
    internal class Link : IComparable<Link>
    {
        public string Url { get; set; }

        public string Date { get; set; }

        public string GetLink()
        {
            var posixTime = DateTime.SpecifyKind(new DateTime(1970, 1, 1), DateTimeKind.Utc);
            return $"Date { posixTime.AddMilliseconds(long.Parse(Date)).ToShortDateString()}";
        }

        public int CompareTo(Link other)
        {
            var posixTime = DateTime.SpecifyKind(new DateTime(1970, 1, 1), DateTimeKind.Utc);
            var time1 = posixTime.AddMilliseconds(long.Parse(other.Date));
            var time2 = posixTime.AddMilliseconds(long.Parse(Date));
            return time2.CompareTo(time1);

        }
    }
}