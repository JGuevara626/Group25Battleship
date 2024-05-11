using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UsernameValidator : MonoBehaviour
{
    public TMP_InputField usernameInputField;
    public TMP_Text validationText;

    private string[] inappropriateWords = { "inappropriate1", "inappropriate2", "inappropriate3" };

    public void CheckUsernameValidity()
    {
        string username = usernameInputField.text;

        if (IsUsernameValid(username))
        {
            //validationText.text = "Username is valid!";
            Debug.Log("Login successful for username: " + username);
        }
        else
        {
            validationText.text = "Username is invalid!";
            return;
        }
    }

    private bool IsUsernameValid(string username)
    {
        foreach (string word in inappropriateWords)
        {
            if (username.ToLower().Contains(word.ToLower()))
            {
                return false;
            }
        }
        return true;
    }
}
