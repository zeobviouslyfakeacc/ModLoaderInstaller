package me.zeobviouslyfakeacc.modloaderinstaller;

public final class Constants {

	private Constants() {
		throw new Error("No instances!");
	}

	public static final String TLD_VERSION = "v1.12";
	public static final String DLL_NAME = "Rewired_Core.dll";
	public static final String XOR_NAME = "Rewired_Core.xor";
	public static final long UNPATCHED_SIZE = 1418752L;
	public static final long PATCHED_SIZE = 1418752L;
	public static final String UNPATCHED_SHA1 = "77e4ea76ec93d357bf9272363700b9c92c99e9e";
	public static final String PATCHED_SHA1 = "ad73dc36f160f9e85f77e8a86d7f64e09e22a2a4";
	public static final String MOD_LOADER_SHA1 = "2185fe8c6b7a97002ce5fb574772544b629b78c1";
	public static final String ERROR_SHA1 = "- ERROR -";

	private static final String WINDOWS_PATH = "<Steam>\\SteamApps\\common\\TheLongDark\\tld_Data\\Managed\\" + DLL_NAME;
	private static final String LINUX_PATH = "<steam>/steamapps/common/TheLongDark/tld_Data/Managed/" + DLL_NAME;
	private static final String MAC_PATH = "<Steam>/SteamApps/common/TheLongDark/tld/Contents/Resources/Data/Managed/" + DLL_NAME;

	public static String getDllPath() {
		if (isWindows()) {
			return WINDOWS_PATH;
		} else if (isMacOs()) {
			return MAC_PATH;
		} else { // Overly inclusive else
			return LINUX_PATH;
		}
	}

	public static boolean isWindows() {
		String os = System.getProperty("os.name").toLowerCase();
		return os.startsWith("windows");
	}

	public static boolean isMacOs() {
		String os = System.getProperty("os.name").toLowerCase();
		return os.startsWith("mac os") || os.startsWith("macos");
	}
}
