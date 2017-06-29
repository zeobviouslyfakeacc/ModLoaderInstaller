package me.zeobviouslyfakeacc.modloaderinstaller;

import javafx.beans.property.StringProperty;
import javafx.concurrent.Task;

import java.io.IOException;
import java.nio.file.Files;
import java.nio.file.Path;

import static me.zeobviouslyfakeacc.modloaderinstaller.FileUtils.applyXOR;

public class Unpatcher extends Task<Void> {

	public static Unpatcher unpatch(Path path, StringProperty messageProperty) {
		Unpatcher unpatcher = new Unpatcher(path);
		messageProperty.bind(unpatcher.messageProperty());
		Thread thread = new Thread(unpatcher);
		thread.start();
		return unpatcher;
	}

	private final Path path;

	private Unpatcher(Path pathToFile) {
		this.path = pathToFile;
	}

	@Override
	protected Void call() throws Exception {
		updateMessage("Unpatching assembly...");

		try {
			applyXOR(path, Constants.UNPATCHED_SIZE);
		} catch (Throwable t) {
			updateMessage("Patching failed. The modded file has been restored.");
			throw t;
		}

		updateMessage("Removing libraries...");

		try {
			uninstallLibraries();
		} catch (Throwable t) {
			updateMessage("The game was successfully patched, but deleting libraries failed.");
		}

		updateMessage("Done!");
		return null;
	}

	private void uninstallLibraries() throws IOException {
		Path modLoaderFile = path.resolveSibling("ModLoader.dll");
		Files.deleteIfExists(modLoaderFile);

		Path harmonyFile = path.resolveSibling("Harmony.dll");
		Files.deleteIfExists(harmonyFile);
	}
}
