using UnityEngine;
using UnityEngine.UI;

namespace AK.Asterovival
{
    public class GameWidget : MonoBehaviour
    {
        [SerializeField] private GameInput _input;
        [SerializeField] private GameEntry _entry;

        [SerializeField] private Text 
            _debugLabel, 
            _scoreLabel;

        [SerializeField] private Image 
            _rotateLeftImg, 
            _rotateRightImg, 
            _thrustImg, 
            _fireImg,
            _laserImg;

        [SerializeField] private Color _activeColor, _color;

        [SerializeField] private GameObject _gameOverPanel;

        private void Update()
        {
            _rotateLeftImg.color = GetColor(_input.LeftAction.IsPressed());
            _rotateRightImg.color = GetColor(_input.RightAction.IsPressed());
            _thrustImg.color = GetColor(_input.ThrustAction.IsPressed());
            _fireImg.color = GetColor(_input.FireAction.IsPressed());
            _laserImg.color = GetColor(_input.LaserAction.IsPressed());

            _debugLabel.text = _entry.ShipDebug;
            _scoreLabel.text = $"Score: {_entry.Score}";
            _gameOverPanel.SetActive(_entry.GameOver);
        }

        private Color GetColor(bool flag) => flag ? _activeColor : _color;
    }
}
