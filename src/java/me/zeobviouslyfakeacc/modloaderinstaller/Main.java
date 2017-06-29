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

		primaryStage.getIcons().add(new Image(Main.class.getResourceAsStream("/icon.png")));
		primaryStage.setMinHeight(300);
		primaryStage.setMinWidth(530);
		primaryStage.setHeight(300);
		primaryStage.setWidth(System.getProperty("os.name").startsWith("Windows") ? 530 : 600);
		primaryStage.setScene(scene);
		primaryStage.setTitle("TLD Mod Loader Installer");
		primaryStage.show();
	}
}
