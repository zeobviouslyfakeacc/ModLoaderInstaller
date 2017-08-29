package me.zeobviouslyfakeacc.modloaderinstaller;

import javafx.application.Application;
import javafx.fxml.FXMLLoader;
import javafx.scene.Parent;
import javafx.scene.Scene;
import javafx.scene.image.Image;
import javafx.stage.Stage;

public class Main extends Application {

	@Override
	public void start(Stage primaryStage) throws Exception {
		Parent root = FXMLLoader.load(Main.class.getResource("MainPanel.fxml"));
		Scene scene = new Scene(root);
		primaryStage.setScene(scene);

		int width = Constants.isWindows() ? 530 : Constants.isMacOs() ? 670 : 600;
		int height = Constants.isWindows() ? 300 : 320;

		primaryStage.getIcons().add(new Image(Main.class.getResourceAsStream("/icon.png")));
		primaryStage.setMinHeight(height);
		primaryStage.setMinWidth(width);
		primaryStage.setHeight(height);
		primaryStage.setWidth(width);
		primaryStage.setResizable(false);
		primaryStage.setTitle("TLD Mod Loader Installer");
		primaryStage.show();
	}
}
