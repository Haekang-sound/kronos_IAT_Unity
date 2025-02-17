using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "NewCheckpointData", menuName = "Game/Checkpoint Data")]
public class CheckpointData : ScriptableObject
{
    public int priority;
    public float healTP;
    public string sceneName;
    public Vector3 SpwanPos;
    public Quaternion SpwanRot;

    private AbilityTree _abilityTree;
    private readonly string r_tpKey = "checkPoint-TP";
    private readonly string r_cpKey = "checkPoint-CP";

    public IEnumerator LoadData()
    {
        yield return SceneController.Instance.StartCoroutine(ScreenFader.FadeSceneOut(ScreenFader.FadeType.Loading));

        yield return SceneController.Instance.StartCoroutine(SceneLoader.Instance.LoadSceneCoroutine(sceneName));

        yield return SceneController.Instance.StartCoroutine(LoadData2());

        yield return SceneController.Instance.StartCoroutine(ScreenFader.FadeSceneIn(ScreenFader.FadeType.Loading));
    }
    public IEnumerator LoadData2()
    {
        /// 플레이어
        // 체크포인트로 이동시킴
        var playerRigidbody = Player.Instance.GetComponent<Rigidbody>();
        playerRigidbody.position = SpwanPos;
        playerRigidbody.rotation = SpwanRot;

        // TP 및 CP 가져오기
        Player.Instance.TP = PlayerPrefs.GetFloat(r_tpKey);
        Player.Instance.CP = PlayerPrefs.GetFloat(r_cpKey);

        // 능력 개방 데이터 가져오기
        if (_abilityTree == null)
        {
			Player.Instance.ResetAbilityData(); // 플레이어 강화 데이터를 초기화
			_abilityTree = FindObjectOfType<AbilityTree>();
        }

        _abilityTree.LoadData(SaveLoadManager.Purpose.checkpoint.ToString());

        yield return null;
    }

    public void SaveData()
    {
        SaveLoadManager.Instance.CurrentCheckpoint = this;

        /// 플레이어
        // TP / CP 저장
        PlayerPrefs.SetFloat(r_tpKey, Player.Instance.TP);
        PlayerPrefs.SetFloat(r_cpKey, Player.Instance.CP);

        // 능력 개방 데이터 저장
        if (_abilityTree == null)
        {
            _abilityTree = FindObjectOfType<AbilityTree>();
        }

        _abilityTree.SaveData(SaveLoadManager.Purpose.checkpoint.ToString());
    }
}
