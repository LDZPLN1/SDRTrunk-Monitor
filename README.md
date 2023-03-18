SDRTrunk Monitor is a Windows only tray application to manage the SDRTrunk process and auto-restart it under certain application failure modes.

You must configure the correct SDRTrunk path in TrayForm.vb before compiling (Yes, I'm too lazy to write code to read a settings file):

    ' PATH TO SDRTRUNK
    Private Shared ReadOnly sdrt_path As String = "C:\SDR\SDRTrunk"

You can also configure the watchdog timer (How often it checks to see if the process is still running) by changing the the following:

    ' WATCHDOG TIMER IN ms
    Private Shared ReadOnly pchecktimer As New Timer(60000)

The timer setting is in ms, so 60000 is once per minute

This app will only monitor SDRTrunk processes spawned by this app, e.g. if you started SDRTrunk from the .bat file, you will not bew able to monitor it or auto-restart on errors.

If it does not auto restart a session, ensure the Auto Restart menu item is selected - it is disabled by default and will only display a message when it thinks SDRTrunk has failed.

If the app detects another instance of SDRTrunk is already running, the Start option will not be available.

I'm sure the code could be cleaner... but I do this for fun and in my spare time... and the main part... it works for me.

Only tested on Windows 11, but it should work on Windows 10 as well.
