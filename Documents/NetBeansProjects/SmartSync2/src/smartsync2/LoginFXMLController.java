/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package smartsync2;

import java.io.IOException;
import java.net.URL;
import java.util.ResourceBundle;
import javafx.event.ActionEvent;
import javafx.fxml.FXML;
import javafx.fxml.FXMLLoader;
import javafx.fxml.Initializable;
import javafx.scene.Scene;
import javafx.scene.text.Text;
import javafx.scene.control.TextField;
import javafx.scene.control.PasswordField;
import javafx.scene.layout.Pane;
import javafx.stage.Stage;

/**
 * FXML Controller class
 *
 * @author 312080
 */
public class LoginFXMLController implements Initializable {

    @FXML private Text signinerror;
    @FXML private TextField userName;
    @FXML private PasswordField passwordField;

    /**
     * Initializes the controller class.
     */
    Stage prevStage;

    @FXML 
        public void handleSigninButtonAction(ActionEvent event) throws IOException {
            
            String usr = userName.getText();
            String Password = passwordField.getText();
            credentialsValidate check = new credentialsValidate();
            boolean credentialValidate = check.validateCredentials(usr, Password);
            
            if (credentialValidate){
                Stage homestage = new Stage();
                homestage.setTitle("Home");
                Pane homePane = null;
                homePane = FXMLLoader.load(getClass().getResource("Home.fxml"));
                Scene scene = new Scene(homePane,1200, 675);
                homestage.setScene(scene);
                
        // Closing the existing Window        
                prevStage = (Stage) userName.getScene().getWindow();
                prevStage.close();
        
        // Opening the Home Screen
                homestage.show();

                signinerror.setText("");
            }
            else{
            signinerror.setText("Invalid Credentials");}
            
    }
    
    @Override
    public void initialize(URL url, ResourceBundle rb) {
        // TODO
    }    
    
}
