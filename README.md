# SkypeStatusChanger
A small utility that:
- change Skype status when user locks Windows 
- via ftp change an image in a site to simulate an online status (like old no longer working mystatus.skype.com)

Developed in c# with Visual Studio 2015, this is a improvement of [savbace "Skype Status Changer"](http://www.codeproject.com/Articles/603969/Skype-Status-Changer)

Improvements added are:
- reduced number of project s (so .dll)
- using nuget to retrieve latest libraries
- inserted the update via FTP feature

The project is released under [The Code Project Open License (CPOL) 1.02](http://www.codeproject.com/info/cpol10.aspx)

*First time app connects to Skype you need to authorize in Skype the connection coming from SkypeStatusChanger*

**Server side**

The ftp connection data (host, username, password) should be set in app.config file

The running skype account will be autodetect by the application. It will look for a subfolder (specified in app.config) and then in a subfolder that mathc the skype username. In that folder should be present [-1, ... 7].png files rapresenting the corresponding status. The software will set the current status as current.png via renaming numbered images. A sample of these file are present in /image subdirectory of the project. If you wish to customize:

        Unknown = -1,
        Offline = 0,
        Online = 1,
        Away = 2,
        NotAvailable = 3,
        DoNotDisturb = 4,
        Invisible = 5,
        LoggedOut = 6,
        SkypeMe = 7,

For autorefreshing images you can use:

1. [Image Autorefresh](https://wordpress.org/plugins/image-autorefresh-shortcode/) plugin for wordpress 
2. You can take a look in the /ServerSide folder to grab some useful javascript

**Notes**

You have to make sure that:

1. Skype is installed
2. In the project Platform Target should be kept as x86 (not any) because it use Skype4COM dll.
3. If any COM error arise trying registering Skype4COM 
   C:\Program Files (x86)\Common Files\Skype>regsvr32 Skype4COM.dll

