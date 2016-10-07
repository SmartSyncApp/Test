/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package smartsync2;

import java.net.URL;
import java.util.ResourceBundle;
import javafx.event.ActionEvent;
import javafx.fxml.FXML;
import javafx.fxml.Initializable;
import javafx.scene.text.Text;
import javafx.scene.control.TextField;
import javafx.scene.control.PasswordField;

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
    
    
    @FXML 
        public void handleSigninButtonAction(ActionEvent event) {
            
            
            
            signinerror.setText("Sign in button pressed");
            
    }
    
    public void initialize(URL url, ResourceBundle rb) {
        // TODO
    }    
    
}
