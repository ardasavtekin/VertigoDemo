using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Events;
using System.Collections.Generic;
using TMPro;

namespace UI.FortuneWheel
{
    public class FortuneWheel : MonoBehaviour
    {
        [Header("References :")] [SerializeField]
        private GameObject linePrefab;

        [SerializeField] private Transform linesParent;
        [SerializeField] private Image wheelImage;
        [SerializeField] private Image indicatorImage;
        [SerializeField] private Sprite bronzeWheelSprite;
        [SerializeField] private Sprite silverWheelSprite;
        [SerializeField] private Sprite goldenWheelSprite;
        [SerializeField] private Sprite bronzeIndicatorSprite;
        [SerializeField] private Sprite silverIndicatorSprite;
        [SerializeField] private Sprite goldenIndicatorSprite;
        [SerializeField] private TextMeshProUGUI spinTypeText;
        [Space] [SerializeField] private Transform PickerWheelTransform;
        [SerializeField] private Transform wheelCircle;
        [SerializeField] private GameObject wheelPiecePrefab;
        [SerializeField] private Transform wheelPiecesParent;

        [Space] [Header("Sounds :")] [SerializeField]
        private AudioSource audioSource;

        [SerializeField] private AudioClip tickAudioClip;
        [SerializeField] [Range(0f, 1f)] private float volume = .5f;
        [SerializeField] [Range(-3f, 3f)] private float pitch = 1f;

        [Space] [Header("Picker wheel settings :")] [Range(1, 20)]
        public int spinDuration = 8;

        [SerializeField] [Range(.2f, 2f)] private float wheelSize = 1f;

        [Space] [Header("Fortune wheel pieces :")]
        public WheelPiece[] bronzeWheelPieces;

        public WheelPiece[] silverWheelPieces;
        public WheelPiece[] goldenWheelPieces;
        public WheelPiece[] wheelPieces;

        // Events
        private UnityAction onSpinStartEvent;
        private UnityAction<WheelPiece> onSpinEndEvent;
        private bool _isSpinning = false;

        public bool IsSpinning
        {
            get { return _isSpinning; }
        }

        private Vector2 pieceMinSize = new Vector2(60f, 80f);
        private Vector2 pieceMaxSize = new Vector2(80f, 100f);
        private int piecesMin = 2;
        private int piecesMax = 12;
        private float pieceAngle;
        private float halfPieceAngle;
        private float halfPieceAngleWithPaddings;
        private double accumulatedWeight;
        private System.Random rand = new System.Random();
        private List<int> nonZeroChancesIndices = new List<int>();
        public WheelType wheelType;

        private void Start()
        {
            wheelPieces = bronzeWheelPieces;
            pieceAngle = 360 / wheelPieces.Length;
            halfPieceAngle = pieceAngle / 2f;
            halfPieceAngleWithPaddings = halfPieceAngle - (halfPieceAngle / 4f);
            Generate();
            CalculateWeightsAndIndices();
            SetupAudio();
        }

        public void SetWheelAndIndicatorView()
        {
            switch (wheelType)
            {
                case WheelType.Bronze:
                    wheelImage.sprite = bronzeWheelSprite;
                    indicatorImage.sprite = bronzeIndicatorSprite;
                    spinTypeText.text = "BRONZE SPIN";
                    spinTypeText.color = new Color(198 / 255f, 134 / 255f, 71 / 255f);
                    wheelCircle.rotation = Quaternion.identity;
                    wheelPieces = bronzeWheelPieces;
                    Generate();
                    break;
                case WheelType.Silver:
                    wheelImage.sprite = silverWheelSprite;
                    indicatorImage.sprite = silverIndicatorSprite;
                    spinTypeText.text = "SILVER SPIN";
                    spinTypeText.color = new Color(138 / 255f, 136 / 255f, 135 / 255f);
                    wheelCircle.rotation = Quaternion.identity;
                    wheelPieces = silverWheelPieces;
                    Generate();
                    break;
                case WheelType.Golden:
                    wheelImage.sprite = goldenWheelSprite;
                    indicatorImage.sprite = goldenIndicatorSprite;
                    spinTypeText.text = "GOLDEN SPIN";
                    spinTypeText.color = new Color(206 / 255f, 152 / 255f, 58 / 255f);
                    wheelCircle.rotation = Quaternion.identity;
                    wheelPieces = goldenWheelPieces;
                    Generate();
                    break;
                default:
                    wheelImage.sprite = bronzeWheelSprite;
                    indicatorImage.sprite = bronzeIndicatorSprite;
                    spinTypeText.text = "BRONZE SPIN";
                    spinTypeText.color = new Color(198 / 255f, 134 / 255f, 71 / 255f);
                    wheelCircle.rotation = Quaternion.identity;
                    wheelPieces = bronzeWheelPieces;
                    Generate();
                    break;
            }
        }

        private void SetupAudio()
        {
            audioSource.clip = tickAudioClip;
            audioSource.volume = volume;
            audioSource.pitch = pitch;
        }

        private void Generate()
        {
            wheelCircle.rotation = Quaternion.identity;
            foreach (Transform child in wheelPiecesParent)
            {
                Destroy(child.gameObject);
            }

            foreach (Transform child in linesParent)
            {
                Destroy(child.gameObject);
            }

            wheelPiecePrefab = InstantiatePiece();
            RectTransform rt = wheelPiecePrefab.transform.GetChild(0).GetComponent<RectTransform>();
            float pieceWidth = Mathf.Lerp(pieceMinSize.x, pieceMaxSize.x,
                1f - Mathf.InverseLerp(piecesMin, piecesMax, wheelPieces.Length));
            float pieceHeight = Mathf.Lerp(pieceMinSize.y, pieceMaxSize.y,
                1f - Mathf.InverseLerp(piecesMin, piecesMax, wheelPieces.Length));
            rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, pieceWidth);
            rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, pieceHeight);
            for (int i = 0; i < wheelPieces.Length; i++) DrawPiece(i);

            //Destroy(wheelPiecePrefab);
        }

        private void DrawPiece(int index)
        {
            WheelPiece piece = wheelPieces[index];
            Transform pieceTrns = InstantiatePiece().transform.GetChild(0);
            pieceTrns.GetChild(0).GetComponent<Image>().sprite = piece.icon;
            pieceTrns.GetChild(1).GetComponent<Text>().text = piece.Amount.ToString();
            Transform lineTrns = Instantiate(linePrefab, linesParent.position, Quaternion.identity, linesParent)
                .transform;
            lineTrns.RotateAround(wheelPiecesParent.position, Vector3.back, (pieceAngle * index) + halfPieceAngle);
            pieceTrns.RotateAround(wheelPiecesParent.position, Vector3.back, pieceAngle * index);
        }

        private GameObject InstantiatePiece()
        {
            return Instantiate(wheelPiecePrefab, wheelPiecesParent.position, Quaternion.identity, wheelPiecesParent);
        }

        public void Spin()
        {
            if (!_isSpinning)
            {
                _isSpinning = true;
                if (onSpinStartEvent != null) onSpinStartEvent.Invoke();
                int index = GetRandomPieceIndex();
                WheelPiece piece = wheelPieces[index];
                if (piece.Chance == 0 && nonZeroChancesIndices.Count != 0)
                {
                    index = nonZeroChancesIndices[Random.Range(0, nonZeroChancesIndices.Count)];
                    piece = wheelPieces[index];
                }

                float angle = -(pieceAngle * index);
                float rightOffset = (angle - halfPieceAngleWithPaddings) % 360;
                float leftOffset = (angle + halfPieceAngleWithPaddings) % 360;
                float randomAngle = Random.Range(leftOffset, rightOffset);
                Vector3 targetRotation = Vector3.back * (randomAngle + 2 * 360 * spinDuration);
                float prevAngle, currentAngle;
                prevAngle = currentAngle = wheelCircle.eulerAngles.z;
                bool isIndicatorOnTheLine = false;
                wheelCircle.DORotate(targetRotation, spinDuration, RotateMode.Fast).SetEase(Ease.InOutQuart).OnUpdate(
                    () =>
                    {
                        float diff = Mathf.Abs(prevAngle - currentAngle);
                        if (diff >= halfPieceAngle)
                        {
                            if (isIndicatorOnTheLine)
                            {
                                audioSource.PlayOneShot(audioSource.clip);
                            }

                            prevAngle = currentAngle;
                            isIndicatorOnTheLine = !isIndicatorOnTheLine;
                        }

                        currentAngle = wheelCircle.eulerAngles.z;
                    }).OnComplete(() =>
                {
                    _isSpinning = false;
                    if (onSpinEndEvent != null) onSpinEndEvent.Invoke(piece);
                    onSpinStartEvent = null;
                    onSpinEndEvent = null;
                });
            }
        }

        public void OnSpinStart(UnityAction action)
        {
            onSpinStartEvent = action;
        }

        public void OnSpinEnd(UnityAction<WheelPiece> action)
        {
            onSpinEndEvent = action;
        }

        private int GetRandomPieceIndex()
        {
            double r = rand.NextDouble() * accumulatedWeight;
            for (int i = 0; i < wheelPieces.Length; i++)
                if (wheelPieces[i]._weight >= r)
                    return i;
            return 0;
        }

        private void CalculateWeightsAndIndices()
        {
            for (int i = 0; i < wheelPieces.Length; i++)
            {
                WheelPiece piece = wheelPieces[i];
                accumulatedWeight += piece.Chance;
                piece._weight = accumulatedWeight;
                piece.Index = i;
                if (piece.Chance > 0) nonZeroChancesIndices.Add(i);
            }
        }

        private void OnValidate()
        {
            if (PickerWheelTransform != null) PickerWheelTransform.localScale = new Vector3(wheelSize, wheelSize, 1f);
        }
    }
}

public enum WheelType
{
    Bronze,
    Silver,
    Golden
}