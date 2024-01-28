using UnityEngine;
using TMPro;

public class MenuManager : MonoBehaviour
{
    public string _version;
    public TMP_Text _versionText;
    public TMP_Text _user;
    public TMP_Text _debug;
    private Animator _anim;
    public TMP_InputField  _userField;
    public TMP_InputField  _pwdField;
    private Connection con;

    void Start()
    {
        _versionText.text = _version;
        _anim = GameObject.Find("GUI").transform.GetChild(0).transform.GetChild(4).GetComponent<Animator>();
    }

    public void Session(){

        if(_anim.GetCurrentAnimatorStateInfo(0).IsName("IN")){
            if(false){ //in construction
                _debug.text = "Success :)";
                _user.text = con.GetUser();
            }
            else{
                _debug.text = "Failed :(";
                _debug.color = new Color(1,0.5f,0.5f);
            }
        }
        else
            _anim.Play("IN");    
    }
}
