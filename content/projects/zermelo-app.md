+++
title = "Schoolrooster voor Zermelo"
tagline = "The Windows 10 app for viewing Zermelo school timetables"
tech = [ "UWP", "XAML", "Template10", "C#" ]
tags = [ "Zermelo", "schoolrooster", "C#", "UWP" ]
color = "#dc1528"
image = "/assets/zermelo-app/phone_schedule.png"
image-alt = "A Windows Phone showing the timetable for June 28th 2017"

[[links]]
name = "Website"
icon = "ios-globe"
url = "http://schoolrooster.arthurrump.com/"
[[links]]
name = "Microsoft Store"
icon = "microsoft"
url = "https://www.microsoft.com/nl-nl/p/schoolrooster-voor-zermelo/9nblggh5fdl2"
[[links]]
name = "GitHub repository"
icon = "github"
url = "https://github.com/arthurrump/Zermelo.App.UWP"
[[links]]
name = "API Helper Library"
icon = "github"
url = "https://github.com/arthurrump/Zermelo.API"
+++

*Schoolrooster voor Zermelo* is an app that allows students and teachers to check their timetables on all their Windows 10 devices. The app works with all schools and organizations that use the Zermelo portal technology to manage the timetabling process. The app is built on the Universal Windows Platform with support for both phone and desktop. An older version is also available for Windows Phone 8.1.

---

## Windows 10 app
The UWP app, built using Template10, is the newest version of *Schoolrooster voor Zermelo*. It supports showing daily timetables for yourself, as well as the option to search for the timetable of a friend, teacher, class or classroom and keep it easily accessible in the hamburger menu. Announcements are also showed on a separate page.

---

## Windows Phone 8.1 app
The first version of *Schoolrooster voor Zermelo* was built for Windows Phone 8.1 using the WinRT SDK. It only supports viewing your own timetable per day, but that was enough to be useful, since this app was the only way to get access to your timetable on a Windows Phone for a long time.

---

## .NET Library
The Zermelo.API package is a general library to access Zermelo timetable data in .NET. It is based on code from the original app and extended to support all features of the UWP app. It also comes with pretty good [documentation](http://zermelo.api.arthurrump.com/).