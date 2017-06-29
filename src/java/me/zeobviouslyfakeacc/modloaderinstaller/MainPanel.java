package me.zeobviouslyfakeacc.modloaderinstaller;

import javafx.beans.binding.Bindings;
import javafx.beans.binding.ObjectBinding;
import javafx.beans.property.BooleanProperty;
import javafx.beans.property.Property;
import javafx.beans.property.SimpleBooleanProperty;
import javafx.beans.property.SimpleObjectProperty;
import javafx.beans.property.StringProperty;
import javafx.concurrent.Task;
import javafx.fxml.FXML;
import javafx.scene.control.Alert;
import javafx.scene.control.Alert.AlertType;
import javafx.scene.control.Button;
import javafx.scene.control.Label;
import javafx.scene.control.TextField;
import javafx.stage.FileChooser;
import javafx.stage.FileChooser.ExtensionFilter;

import java.io.File;
import java.nio.file.Files;
import java.nio.file.Path;

import static me.zeobviouslyfakeacc.modloaderinstaller.Constants.DLL_NAME;
import static me.zeobviouslyfakeacc.modloaderinstaller.Constants.TLD_VERSION;
import static me.zeobviouslyfakeacc.modloaderinstaller.FileUtils.hashFile;

public class MainPanel {

	public final Property<Path> selectedFile = new SimpleObjectProperty<>(null);
	public final Property<FileStatus> fileStatusProperty = new SimpleObjectProperty<>(FileStatus.NO_FILE);
	public final BooleanProperty working = new SimpleBooleanProperty(false);

	public TextField fileTextField;
	public Button fileSelectButton;
	public Label fileStatusLabel;
	public Label progressStatusLabel;
	public Button patchButton;
	public Label instructionsLabel;
	public ObjectBinding<FileStatus> fileStatusBinding;

	@FXML
	private void initialize() {
		// Replace constants in instructions label
		instructionsLabel.setText(instructionsLabel.getText()
				.replace("${version}", TLD_VERSION)
				.replace("${dll_name}", DLL_NAME));

		// Text field not selectable and displays selected file
		fileTextField.setFocusTraversable(false);
		fileTextField.textProperty().bind(Bindings.createStringBinding(() -> {
			if (selectedFile.getValue() == null) return "";
			return selectedFile.getValue().toAbsolutePath().toString();
		}, selectedFile));

		// File status depends on selected file
		fileStatusBinding = Bindings.createObjectBinding(this::calculateHashes, selectedFile);
		fileStatusProperty.bind(fileStatusBinding);

		// File status label responds to selected file status
		fileStatusLabel.textProperty().bind(Bindings.createStringBinding(
				() -> fileStatusProperty.getValue().getDisplayName(), fileStatusProperty));
		fileStatusLabel.textFillProperty().bind(Bindings.createObjectBinding(
				() -> fileStatusProperty.getValue().getDisplayColor(), fileStatusProperty));

		// File select button should be focused
		fileSelectButton.requestFocus();

		// Patch button should only be enabled if file is valid and not currently patching
		patchButton.disableProperty().bind(working.or(Bindings.createBooleanBinding(
				() -> !fileStatusProperty.getValue().isValid(), fileStatusProperty)));
		patchButton.textProperty().bind(Bindings.createStringBinding(
				() -> fileStatusProperty.getValue().getButtonText(),
				fileStatusProperty));
	}

	private FileStatus calculateHashes() {
		Path path = selectedFile.getValue();
		if (path == null) return FileStatus.NO_FILE;

		String dllHash = hashFile(path);
		String modLoaderHash = hashFile(path.resolveSibling("ModLoader.dll"));
		return FileStatus.forHash(dllHash, modLoaderHash);
	}

	public void selectFile() {
		selectedFile.setValue(null);

		FileChooser fileChooser = new FileChooser();
		fileChooser.setTitle("Select \"" + DLL_NAME + "\"");
		ExtensionFilter filter = new ExtensionFilter(DLL_NAME, DLL_NAME);
		fileChooser.getExtensionFilters().add(filter);
		fileChooser.setSelectedExtensionFilter(filter);

		File selected = fileChooser.showOpenDialog(null);

		if (selected == null || !selected.exists() || !selected.isFile()) return;
		if (!selected.canRead() || !selected.canWrite()) {
			selectedFile.setValue(null);
			error("Read error", null, "The file you selected cannot be read from or written to.");
			return;
		}

		selectedFile.setValue(selected.toPath());
	}

	public void patchOrUnpatch() {
		if (working.get()) return;
		Path path = selectedFile.getValue();
		if (path == null || !Files.isRegularFile(path) || !Files.isWritable(path)) {
			error("Patch error", null, "The specified file was invalid or was not writable.");
			return;
		}

		FileStatus fileStatus = fileStatusProperty.getValue();
		StringProperty messageProperty = progressStatusLabel.textProperty();

		Task<Void> task;
		switch (fileStatus) {
			case VALID_UNPATCHED:
				task = Patcher.patch(path, messageProperty);
				break;
			case VALID_OUTDATED:
				task = Patcher.update(path, messageProperty);
				break;
			case VALID_PATCHED:
				task = Unpatcher.unpatch(path, messageProperty);
				break;
			default:
				return;
		}

		working.set(true);
		task.setOnSucceeded((ignored) -> {
			fileStatusBinding.invalidate();
			working.set(false);
		});
	}

	private static void error(String title, String header, String text) {
		Alert alert = new Alert(AlertType.ERROR);
		alert.setTitle(title);
		alert.setHeaderText(header);
		alert.setContentText(text);

		alert.showAndWait();
	}
}
