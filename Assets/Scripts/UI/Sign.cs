using UnityEngine;
using UnityEngine.InputSystem;

public class Sign : MonoBehaviour
{
    Red red;
    Animator anim;
    AudioDefination audioPlayer;
    IInteractable TargetItem;
    public Transform Player;
    public GameObject signSprite;
    public bool canPress;

    private void Awake() 
    {
        anim =  signSprite.GetComponent<Animator>();
        audioPlayer = GetComponent<AudioDefination>();
        red = new Red();
        red.Enable();
        red.GameSystem.Confirm.started += ChestOpen;
    }

   

    private void OnEnable() 
    {
        //  一个固定的写法，是inputSystem帮我们写好的方法
        // 代表的是控制器改变的时候发生的变化
        InputSystem.onActionChange += OnActionChange;
    }

    void Update()
    {
        signSprite.GetComponent<SpriteRenderer>().enabled = canPress;
        signSprite.transform.localScale = Player.localScale;
    }

    /// <summary>
    /// 控制器转换的检测
    /// </summary>
    /// <param name="obj">返回的物体（可以强制转化为所有的类型）</param>
    /// <param name="actionChange">改变后的值</param>
    private void OnActionChange(object obj, InputActionChange actionChange)
    {
        if(actionChange == InputActionChange.ActionStarted)
        {
            // print(((InputAction)obj).activeControl.device);
            var ctrl = ((InputAction)obj).activeControl.device;

            switch(ctrl.device)
            {
                case Keyboard:
                    anim.Play("keyboard");
                    break;
                case Gamepad:
                    anim.Play("Xbox");
                    break;
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D other) 
    {
        if(other.CompareTag("Interactable"))
        {
            TargetItem = other.GetComponent<IInteractable>();
            
            canPress = true;
        }
    }
    /// <summary>
    /// 所有按下E键的指令都在这里，执行E键的指令
    /// </summary>
    /// <param name="context"></param>
     private void ChestOpen(InputAction.CallbackContext context)
    {
        if(canPress)
        {
            TargetItem.TriggerAction();
            canPress = false;
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        canPress = false;
    }
}
