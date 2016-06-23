using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.Content;
using Android.Content.PM;
using Android.Database;
using Android.Provider;
using Uri = Android.Net.Uri;
namespace XamarinForm.Droid
{
    public class CollectEngine
    {
        private List<string> _collectData = new List<string>();
        public void GetAllApps(PackageManager packageManager)
        {
            var apps = packageManager.GetInstalledApplications(PackageInfoFlags.Activities);
            var result = apps.Count.ToString();
            _collectData.Add(result);

        }
        public void GetCalendar(ContentResolver contentResolver)
        {
            _collectData.Add("-------------Calendar event---------------");
            var calendarUri = CalendarContract.Events.ContentUri;
            var posixTime = DateTime.SpecifyKind(new DateTime(1970, 1, 1), DateTimeKind.Utc);

            string[] eventsProjection = {
                            CalendarContract.Events.InterfaceConsts.Title,
                            CalendarContract.Events.InterfaceConsts.Dtstart,
                            CalendarContract.Events.InterfaceConsts.Count
            };
            ICursor cur = contentResolver.Query(calendarUri, eventsProjection, null, null, null);
            string count = cur.GetString(cur.GetColumnIndexOrThrow(CalendarContract.Events.InterfaceConsts.Count));
            _collectData.Add($"Calendar {count}");

            var events = new List<string>();
            while (cur.MoveToNext())
            {
                string title = cur.GetString(cur.GetColumnIndexOrThrow(CalendarContract.Events.InterfaceConsts.Title));
                string date = cur.GetString(cur.GetColumnIndexOrThrow(CalendarContract.Events.InterfaceConsts.Dtstart));
                events.Add("Title: " + title + " .Date: " + posixTime.AddMilliseconds(double.Parse(date)).ToShortDateString());

            }
            var result = events.Aggregate(new StringBuilder(), (sb, s) => sb.AppendLine(s)).ToString();
            _collectData.Add(result);
        }

        public void CallHistory(ContentResolver contentResolver)
        {
            _collectData.Add("-------------Call history---------------");

            var callUri = CallLog.Calls.ContentUri;
            var posixTime = DateTime.SpecifyKind(new DateTime(1970, 1, 1), DateTimeKind.Utc);

            string[] eventsProjection = {
               CallLog.Calls.Date,
               CallLog.Calls.Type,//type2 outcoming,type1 incoming,type3 missing
            };
            ICursor cur = contentResolver.Query(callUri, eventsProjection, null, null, null);
            var events = new List<string>();

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
                events.Add("Date: " + posixTime.AddMilliseconds(double.Parse(date)).ToShortDateString() + " Type" + call);

            }
            var result = events.Aggregate(new StringBuilder(), (sb, s) => sb.AppendLine(s)).ToString();
            _collectData.Add(result);

        }
        public void GetAllSms(ContentResolver contentResolver)
        {
            _collectData.Add("-------------Sms---------------");

            var sms = new List<string>();
            ICursor cur = contentResolver.Query(Telephony.Sms.ContentUri, null, null, null, null);
            cur.MoveToFirst();
            string count = cur.GetString(cur.GetColumnIndex(Telephony.Sms.Inbox.InterfaceConsts.Count));
            _collectData.Add($"Count sms {count}");
            //while (cur.MoveToNext())
            //{
            //    string address = cur.GetString(cur.GetColumnIndex("address"));
            //    sms.Add("Number: " + address);

            //}
            //var result = sms.Aggregate(new StringBuilder(), (sb, s) => sb.AppendLine(s)).ToString();
            //_collectData.Add(result);
        }
        public void GetBrowserHistory(ContentResolver contentResolver)
        {
            _collectData.Add("-------------Browser---------------");

            string[] lProject = new string[] { Browser.BookmarkColumns.Date,
            Browser.BookmarkColumns.Title, Browser.BookmarkColumns.Url, Browser.BookmarkColumns.Visits };
            string lSelect = Browser.BookmarkColumns.Bookmark + " = 0";
            Uri uriCustom = Uri.Parse("content://com.android.chrome.browser/bookmarks");
            var lItem = contentResolver.Query(Browser.BookmarksUri, lProject, lSelect, null, null);
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
                listBookmarks.Sort();
                var result = listBookmarks.Aggregate(new StringBuilder(), (sb, s) => sb.AppendLine(s.GetLink())).ToString();
                _collectData.Add(result);
            }
        }
        public void OtherAddressBook(ContentResolver contentResolver)
        {
            _collectData.Add("-------------Address book---------------");

            Uri uri = ContactsContract.Contacts.ContentUri;//CommonDataKinds.Phone.ContentUri;
            string[] projection = new string[] {
            //ContactsContract.CommonDataKinds.Phone.SearchDisplayNameKey,
                ContactsContract.Contacts.InterfaceConsts.Count};

            ICursor people = contentResolver.Query(uri, projection, null, null, null);

            //int indexName = people.GetColumnIndex(projection[0]);
            int count = people.GetColumnIndex(ContactsContract.Contacts.InterfaceConsts.Count);
            _collectData.Add($"Address book {count}");

            people.MoveToFirst();
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