﻿using UnityEngine;
using View.Items;

namespace View.Control
{
    public class MenuRotator : MonoBehaviour
    {
        public ButtonScript TopButton;
        public ButtonScript BottomButton;

        private GameObject _topButton;
        private GameObject _bottomButton;

        private float _currentRotation;

        private void Awake()
        {
            _topButton = TopButton.gameObject;
            _bottomButton = BottomButton.gameObject;
        }

        private void Start()
        {
            _bottomButton.GetComponent<BoxCollider>().enabled = false;
        }

        public void ShowSettingsButtons()
        {
            // TODO: make configurable
            const float time = 0.8f;
            const float delay = 0.2f;
            
            _bottomButton.GetComponent<BoxCollider>().enabled = false;

            LeanTween.rotateAroundLocal(gameObject, Vector3.right, 180f - _currentRotation, time)
                .setDelay(delay)
                .setEase(LeanTweenType.easeInOutBack)
                .setOnComplete(() => {
                    _bottomButton.GetComponent<BoxCollider>().enabled = true;
                    _currentRotation = 180f;
                });
        }

        public void ShowLevelButtons()
        {
            // TODO: make configurable
            const float time = 0.8f;
            const float delay = 0.2f;
            
            _topButton.GetComponent<BoxCollider>().enabled = false;

            LeanTween.rotateAroundLocal(gameObject, Vector3.right, _currentRotation, time)
                .setDelay(delay)
                .setEase(LeanTweenType.easeInOutBack)
                .setOnComplete(() => {
                    _topButton.GetComponent<BoxCollider>().enabled = true;
                    
                    _currentRotation = 0f;
                });
        }
    }
}
