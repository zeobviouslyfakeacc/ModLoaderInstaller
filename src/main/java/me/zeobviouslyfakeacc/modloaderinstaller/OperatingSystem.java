package me.zeobviouslyfakeacc.modloaderinstaller;

import java.nio.file.Path;
import java.nio.file.Paths;

public enum OperatingSystem {

	WINDOWS {
		@Override
		public String getManagedPath() {
			return "tld_Data\\Managed";
		}

		@Override
		public String getExecutableName() {
			return "tld.exe";
		}

		@Override
		public String getDefaultGamePath() {
			return "C:\\Program Files (x86)\\Steam\\SteamApps\\common\\TheLongDark";
		}
	},

	MAC_OS {
		@Override
		public String getManagedPath() {
			return "tld.app/Contents/Resources/Data/Managed";
		}

		@Override
		public String getExecutableName() {
			return "tld.app";
		}

		@Override
		public String getDefaultGamePath() {
			return "~/Library/Application Support/Steam/SteamApps/common/TheLongDark";
		}
	},

	LINUX {
		@Override
		public String getManagedPath() {
			return "tld_Data/Managed";
		}

		@Override
		public String getExecutableName() {
			return "tld.x86";
		}

		@Override
		public String getDefaultGamePath() {
			return "~/.steam/steam/steamapps/common/TheLongDark";
		}

		@Override
		public String[] getExtensionFilter() {
			return new String[] {"tld.x86", "tld.x86_64"};
		}
	};

	public static OperatingSystem getCurrent() {
		String os = System.getProperty("os.name").toLowerCase();
		if (os.startsWith("windows")) {
			return WINDOWS;
		} else if (os.startsWith("mac os") || os.startsWith("macos")) {
			return MAC_OS;
		} else {
			return LINUX;
		}
	}

	public abstract String getManagedPath();

	public abstract String getExecutableName();

	public abstract String getDefaultGamePath();

	public String[] getExtensionFilter() {
		return new String[] {getExecutableName()};
	}

	public Path getDLLPath(Path executablePath) {
		return executablePath.resolveSibling(getManagedPath()).resolve(Constants.DLL_NAME);
	}

	public Path getDefaultExecutablePath() {
		String gamePath = getDefaultGamePath();
		if (gamePath.startsWith("~")) {
			gamePath = System.getProperty("user.home") + gamePath.substring(1);
		}
		return Paths.get(gamePath).resolve(getExecutableName());
	}

	public Path getModsDirectory(Path assemblyPath) {
		int levelsUp = (this == MAC_OS) ? 5 : 2;
		Path modsDir = assemblyPath;
		for (int i = 0; i < levelsUp; ++i) {
			modsDir = modsDir.getParent();
		}
		return modsDir.resolveSibling("mods");
	}
}
