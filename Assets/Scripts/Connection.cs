using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Connection : MonoBehaviour
{
    private bool _isConnected;
    private string _user;
    private string _pwd;

    //in construction
    
    public string GetUser(){
        return _user;
    }
    public bool Success(){
        return _isConnected;
    }
}
