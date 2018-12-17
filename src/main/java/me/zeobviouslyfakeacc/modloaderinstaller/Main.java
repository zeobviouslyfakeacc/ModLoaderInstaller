package me.zeobviouslyfakeacc.modloaderinstaller;

import javafx.application.Application;

public class Main {

	public static void main(String[] args) {
		if (OperatingSystem.getCurrent() == OperatingSystem.LINUX) {
			System.setProperty("prism.lcdtext", "false");
		}

		Application.launch(MainApplication.class, args);
	}
}
