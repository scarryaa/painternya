package PainterNya;

import javafx.application.Platform;
import javafx.fxml.FXML;
import javafx.fxml.FXMLLoader;
import javafx.scene.Scene;
import javafx.scene.control.Button;
import javafx.scene.control.TextField;
import javafx.stage.Stage;
import javafx.stage.StageStyle;

import java.io.IOException;

public class NewFileDialogController {

    @FXML
    private TextField fileWidth;
    @FXML
    private TextField fileHeight;
    @FXML
    private Button cancelButton;
    @FXML
    private Button okButton;

    public int newFileWidth;
    public int newFileHeight;

    public void onOK()
    {
        newFileWidth = Integer.parseInt(fileWidth.getText());
        newFileHeight = Integer.parseInt(fileHeight.getText());
        Stage stage = (Stage)cancelButton.getScene().getWindow();
        stage.close();
    }

    public void onCancel() {
        newFileWidth = 0;
        newFileHeight = 0;
        Stage stage = (Stage)cancelButton.getScene().getWindow();
        stage.close();
    }
}
