package me.zeobviouslyfakeacc.modloaderinstaller;

import javafx.application.Application;
import javafx.fxml.FXMLLoader;
import javafx.scene.Parent;
import javafx.scene.Scene;
import javafx.scene.image.Image;
import javafx.stage.Stage;

public class Main extends Application {

	public static void main(String[] args) {
		if (OperatingSystem.getCurrent() == OperatingSystem.LINUX) {
			System.setProperty("prism.lcdtext", "false");
		}

		Application.launch(Main.class, args);
	}

	@Override
	public void start(Stage primaryStage) throws Exception {
		Parent root = FXMLLoader.load(Main.class.getResource("MainPanel.fxml"));
		Scene scene = new Scene(root);
		primaryStage.setScene(scene);

		primaryStage.getIcons().add(new Image(Main.class.getResourceAsStream("/icon.png")));
		primaryStage.sizeToScene();
		primaryStage.setResizable(false);
		primaryStage.setTitle("TLD Mod Loader Installer");
		primaryStage.show();
	}
}
