SDRTrunk Monitor is a Windows only tray application to manage the SDRTrunk process and auto-restart it under certain application failure modes.

This app will only monitor SDRTrunk processes spawned by this app, e.g. if you started SDRTrunk from the .bat file, you will not be able to monitor it or auto-restart on errors.

The first time you run it, or if the SDRTrunk path is not known, you will be prompted to open the settings dialog.

If the app detects another instance of SDRTrunk is already running, the Start option will not be available.

Settings Overview:
* SDRTrunk Path - This should point to the base path for SDRTrunk (the folder that holds the bin, conf, legal, lib folders, etc.)
* SDRTrunk Version - Select the version you are running. This setting affects the parameters passed to the Java executable
* External Command - (Optional) An external command that can be run in between restarts of SDRTrunk. Enclose any options that have spaces in quotes
* Poll timer - How often the app polls the SDRTrunk process to verify it is still running.

Toggle Menu Options:
* Auto Restart - Force an automated restart of SDRTrunk if the process dies or an error is detected
* External Command - Execute the external command specified in the Settings dialog in between restarts of SDRTrunk - it will not run between manual stop/starts

I'm sure the code could be cleaner... but I do this for fun and in my spare time.

Only tested on Windows 11, but it should work on Windows 10 as well.
