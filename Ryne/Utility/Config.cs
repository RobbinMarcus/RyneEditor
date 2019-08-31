using IniGenerator;

namespace Ryne.Utility
{
	public class Config : Ini
	{
		// Graphics
		public System.Int32 Width 
		{ 
			get => System.Int32.Parse(IniFile.GetValue("Graphics", "Width")); 
			set => IniFile.SetValue("Graphics", "Width", value.ToString()); 
		}
		public System.Int32 Height 
		{ 
			get => System.Int32.Parse(IniFile.GetValue("Graphics", "Height")); 
			set => IniFile.SetValue("Graphics", "Height", value.ToString()); 
		}
		public System.Boolean Fullscreen 
		{ 
			get => System.Boolean.Parse(IniFile.GetValue("Graphics", "Fullscreen")); 
			set => IniFile.SetValue("Graphics", "Fullscreen", value.ToString()); 
		}
		public System.Boolean BorderlessFullscreen 
		{ 
			get => System.Boolean.Parse(IniFile.GetValue("Graphics", "BorderlessFullscreen")); 
			set => IniFile.SetValue("Graphics", "BorderlessFullscreen", value.ToString()); 
		}
		public System.Boolean UseFixedTimeStep 
		{ 
			get => System.Boolean.Parse(IniFile.GetValue("Graphics", "UseFixedTimeStep")); 
			set => IniFile.SetValue("Graphics", "UseFixedTimeStep", value.ToString()); 
		}
		public System.Single FixedTimeStep 
		{ 
			get => System.Single.Parse(IniFile.GetValue("Graphics", "FixedTimeStep")); 
			set => IniFile.SetValue("Graphics", "FixedTimeStep", value.ToString()); 
		}
		public System.Boolean NoRendering 
		{ 
			get => System.Boolean.Parse(IniFile.GetValue("Graphics", "NoRendering")); 
			set => IniFile.SetValue("Graphics", "NoRendering", value.ToString()); 
		}
		public System.Boolean CaptureTimingProperties 
		{ 
			get => System.Boolean.Parse(IniFile.GetValue("Graphics", "CaptureTimingProperties")); 
			set => IniFile.SetValue("Graphics", "CaptureTimingProperties", value.ToString()); 
		}

		// EditorSettings
		public System.String WorkingDirectory 
		{ 
			get => IniFile.GetValue("EditorSettings", "WorkingDirectory"); 
			set => IniFile.SetValue("EditorSettings", "WorkingDirectory", value); 
		}
		public System.Single GridAlignment 
		{ 
			get => System.Single.Parse(IniFile.GetValue("EditorSettings", "GridAlignment")); 
			set => IniFile.SetValue("EditorSettings", "GridAlignment", value.ToString()); 
		}
		public System.Single AngleAlignment 
		{ 
			get => System.Single.Parse(IniFile.GetValue("EditorSettings", "AngleAlignment")); 
			set => IniFile.SetValue("EditorSettings", "AngleAlignment", value.ToString()); 
		}
		public System.Single ScaleAlignment 
		{ 
			get => System.Single.Parse(IniFile.GetValue("EditorSettings", "ScaleAlignment")); 
			set => IniFile.SetValue("EditorSettings", "ScaleAlignment", value.ToString()); 
		}

		public Config(string filename = "Config.ini") : base(filename)
		{
		}

		protected override void GenerateDefaultIni()
		{
			IniFile.SetValue("Graphics", "Width", "1280");
			IniFile.SetValue("Graphics", "Height", "720");
			IniFile.SetValue("Graphics", "Fullscreen", "False");
			IniFile.SetValue("Graphics", "BorderlessFullscreen", "False");
			IniFile.SetValue("Graphics", "UseFixedTimeStep", "False");
			IniFile.SetValue("Graphics", "FixedTimeStep", "0.01666667");
			IniFile.SetValue("Graphics", "NoRendering", "False");
			IniFile.SetValue("Graphics", "CaptureTimingProperties", "True");
			IniFile.SetValue("EditorSettings", "WorkingDirectory", "Content/");
			IniFile.SetValue("EditorSettings", "GridAlignment", "0.125");
			IniFile.SetValue("EditorSettings", "AngleAlignment", "5");
			IniFile.SetValue("EditorSettings", "ScaleAlignment", "0.125");
		}
	}
}
