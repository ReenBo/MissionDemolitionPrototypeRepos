using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Для работы с UI

public enum GameMode // Это способ определить именнованые числа в C#
{
    idle,
    playing,
    levelEnd
}

public class MissionDemolition : MonoBehaviour
{
    static private MissionDemolition S; // Скрытый объект одиночка

    [Header("Set in Inspector")]
    public Text uitLevel; // Ссылка на объект UIText_Level
    public Text uitShots; // Ссылка на объект UIText_Shots
    public Text uitButton; // Ссылка на дочерний объект Text

    public Vector3 castlePos; // Местоположение замка
    public GameObject[] castles; // Массив замков

    [Header("Set Dinamically")]
    public int level; // Текущий уровень
    public int levelMax; // Количество уровней
    public int shotsTeken;
    public GameObject castle; // Текущий замок
    public GameMode mode = GameMode.idle;
    public string showing = "Show Slingshots"; // Режим FollowCam

    void Start() {
        S = this; // Определить объект-одиночку

        level = 0;
        levelMax = castles.Length;
        StartLevel();
    }

    void StartLevel() {
        // Уничтожить замок если он существует
        if(castle != null) {
            Destroy(castle);
        }

        // Уничтожить прежние снаряды если они существуют
        GameObject[] gos = GameObject.FindGameObjectsWithTag("Projectile");
        foreach (GameObject pTeam in gos) {
            Destroy(pTeam);
        }

        // Создать новый замок
        castle = Instantiate(castles[level]);
        castle.transform.position = castlePos;
        shotsTeken = 0;

        // Переустановить камеру в начальную позицию
        SwitchView("Show Both");
        ProjectileLine.S.Clear();

        // Сбросить цель
        Goal.goalMet = false;

        UpdateGUI();

        mode = GameMode.playing;
    }

    void UpdateGUI() {
        // Показать данные в элементах ПИ
        uitLevel.text = "Level: "+(level+1)+" of " + levelMax;
        uitShots.text = "Shots Taken: " + shotsTeken;
    }

    void Update() {
        UpdateGUI();

        // Проверить завершение уровня 
        if((mode == GameMode.playing) && Goal.goalMet) {
            // Изменить режим, чтобы прекратить проверку завершения уровня
            mode = GameMode.levelEnd;
            // Уменьшить масштаб
            SwitchView("Show Both");
            // Начать новый уровень через 2 секунды
            Invoke("NextLevel", 2f);
        } 
    }

    void NextLevel() {
        level++;
        if(level == levelMax) {
            level = 0;
        }
        StartLevel();
    }

    public void SwitchView(string eView = "") {
        if(eView == "") {
            eView = uitButton.text;
        }
        showing = eView;
        switch (showing) {
            case "Show Slingshot":
                FollowCam.POI = null;
                uitButton.text = "Show Castle";
                break;

            case "Show Castle":
                FollowCam.POI = S.castle;
                uitButton.text = "Show Both";
                break;

            case "Show Both":
                FollowCam.POI = GameObject.Find("ViewBoth");
                uitButton.text = "Show Slingshot";
                break;
        }
    }

    // Статический метод, позволяющий из любого кода увеличить shotsTeken
    public static void ShotFired() {
        S.shotsTeken++;
    }
}
