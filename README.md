This is a feeder project for vJoy. It is meant to be used with CS:GO to allow faster than normal walking speeds.

It is based upon the sample feeder project in the vJoy SDK. Large portions of code will be very similar to that project since those portions worked just fine. Additionally, there are some code snippets/classes/methods/etc from the internet. Their respective authors have been listed in comments inside the source code next to their code. The rest of the code was written by myself, lopezk38 (Nitroge on Steam).

Theory:

While pressing the walk key in CS:GO, the game caps your character to a maximum movement speed of 130 units/s. Most people think that the walk key also makes your character stop making footstep sounds, but the walk key by itself does not actually do this. In reality, there is a movement speed threshold of 135 units/s where if your character is moving slower than it, they will not make any footstep noises. That means that if you could find a way to consistently move at 134 units/s, your character will move faster than walking speed but will still not make any noise. Furthermore, each weapon in CS:GO has a different walk movement speed. The knife is right up there at 130 units/s while most rifles are around 110ish units/s. The Negev (as of time of writing) has a walking movement speed of around 70 units/s, extremely slow. 

CS:GO has support for controllers built in since it was originally intended to be a port of CS:S to the PS3 and X360. This remains in the current day PC version. Player movement is handled in an analog fashion because of this - Pressing WASD is simply equivalent to pushing an analog stick all the way in any direction. This means that if you were to plug in a controller you would be able to finely control your characters movement speed by pushing the analog stick partway. This program takes advantage of this to convert a keyboard button press to a simulated analog stick which has been pushed exactly to the point where your character moves at 125 units/s. 

The only real downside I found to this method is that if you turn your mouse too quickly (such as to flick at a head at the edge of your screen), you could unintentionally strafe. Strafing in most Source engine games including CS:GO causes your character to gain speed (Which is why strafe jumps are used to jump further than normal distances or to gain speed while bhopping) which will put you above 135 units/s if you aren't careful. Make slow turns or hold the walk key while making sharp turns to avoid this.

This is the first fully functional build

External Dependancies:

	vJoy:
	https://sourceforge.net/projects/vjoystick/files/latest/download
	
	.NET Framework 4
	https://www.microsoft.com/en-us/download/details.aspx?id=17718
	
	
Installation:

	Step 1. Install dependancies.
	Step 2. "Run C:\Program Files\vJoy\x64\vJoyConf.exe" and make sure the X and Y axes are enabled. Disable everything else. Make sure Enable vJoy is checked.
	Step 3. Run the binary in "csgoWalk\bin\Debug\csgoWalk.exe"
	Step 4. Copy "analogWalk.cfg" to "<Your CS:GO install directory>\csgo\cfg\"
	Step 5. Add "exec analogWalk.cfg" to your autoexec.cfg or run it in console whenever you want to use the binds
	
Other notes:
	You can use joy_forwardthreshold "0.1" to adjust the speed at which you move. A value of "0.1" will make you move at 125 units/second while "0.15" will make you move at 110ish units/s
	
	By default, the keybinds are WASD and overrides the default CS:GO walking bind with it's own