/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package smartsync2;

/**
 *
 * @author 312080
 */
public class credentialsValidate {
    
      public boolean validateCredentials(String usr, String pwd) {

        if (usr.equals("a") && pwd.equals("a")) {
            return true;
        } else {
            return false;
        }
    }

}
