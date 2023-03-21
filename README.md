SDRTrunk Monitor is a Windows only tray application to manage the SDRTrunk process and auto-restart it under certain application failure modes.

This app will only monitor SDRTrunk processes spawned by this app, e.g. if you started SDRTrunk from the .bat file, you will not be able to monitor it or auto-restart on errors.

The first time you run it, if the SDRTRunk path is not known, you will be prompted to open the settings dialog and set it's loctation and version. You also set the watchdog timer in seconds (how often the app checks to see that the process is still running)

If it does not auto restart a session, ensure the Auto Restart menu item is selected - it is disabled by default and will only display a message when it thinks SDRTrunk has failed. When you change the Auto Restart option on the tray menu, its state will be saved and remembered the next time you run the app.

If the app detects another instance of SDRTrunk is already running, the Start option will not be available.

I'm sure the code could be cleaner... but I do this for fun and in my spare time.

Only tested on Windows 11, but it should work on Windows 10 as well.
