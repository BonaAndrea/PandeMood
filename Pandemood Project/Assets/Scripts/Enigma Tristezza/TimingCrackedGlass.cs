using System.Collections;
using Scripts.PlayerScripts;
using UnityEngine;
using UnityEngine.Serialization;

namespace Enigma_Tristezza
{
    public class TimingCrackedGlass : MonoBehaviour
    {
        public float timeToStartCrack = 10f;
        public float timeToAddCrack = 7f;
        [FormerlySerializedAs("CrackedGlass")] public GameObject[] crackedGlass;
        [FormerlySerializedAs("CrackGlassCharacter")] public GameObject crackGlassCharacter;
        [FormerlySerializedAs("WaterMirrorSurface")] public GameObject waterMirrorSurface;
        [FormerlySerializedAs("WaterSurface")] public Transform waterSurface;
        [FormerlySerializedAs("CharacterFoot")] public Transform characterFoot;
        [FormerlySerializedAs("Character")] public GameObject character;
        [FormerlySerializedAs("RocksUnderWater")] public GameObject rocksUnderWater;
        [FormerlySerializedAs("TeleportPosition")] public Transform teleportPosition;
        [FormerlySerializedAs("camera")] [FormerlySerializedAs("Camera")] public GameObject internalCamera;

        //AUDIO
        [FormerlySerializedAs("CrackGlass")] public AudioSource crackGlass;
        [FormerlySerializedAs("FinalCrack")] public AudioSource finalCrack;

        //TRANSITION FLASH
        [FormerlySerializedAs("FlashImage")] public GameObject flashImage;

        bool _startCrack;
        int _n;

        // Start is called before the first frame update
        void Start()
        {
            rocksUnderWater.SetActive(false);
            crackGlass.Stop();
            finalCrack.Stop();
            StartCoroutine(StartCrack());
        }

        private IEnumerator StartCrack()
        {
            yield return new WaitForSeconds(timeToStartCrack);
            _startCrack = true;
            StartCoroutine(StartCracking());
        }

        //OGNI TOT SECONDI ROMPE UN NUOVO VETRO
        private IEnumerator StartCracking()
        {

            while (_startCrack)
            {
                yield return new WaitForSeconds(timeToAddCrack);
                if (_n < crackedGlass.Length)
                {
                    crackedGlass[_n].SetActive(true);
                    crackGlass.Play();
                }
                _n++;
                if (_n>crackedGlass.Length)
                {
                    _startCrack = false;
                    //qui compare il crack del personaggio
                    var position = characterFoot.position;
                    crackGlassCharacter.transform.position = new Vector3 (position.x, position.y+0.08f, position.z);
                    crackGlassCharacter.SetActive(true); //crack buco
                    finalCrack.Play();
                    Destroy(waterMirrorSurface.GetComponent<Collider>());

                    StartCoroutine(ChangeCharacterPosition()); //TELETRASPORA PERSONAGGIO
                }
            }
        }

        private IEnumerator ChangeCharacterPosition()
        {
            yield return new WaitForSeconds(0.1f);
            flashImage.GetComponent<Flash>().doCameraFlash = true; //effetto flash
            //GameObject character=Instantiate(Character, TeleportPosition.position, Quaternion.Euler(0,180,0));
            //Destroy(Character);
            character.transform.position = teleportPosition.position;
            var swimBehaviour = character.GetComponent<SwimBehaviour>();
            swimBehaviour.checkWater = true;
            swimBehaviour.waterSurface = new RectTransform();
            swimBehaviour.waterSurfacePosition = 50000f;
        
            Destroy(waterMirrorSurface);
            var position = waterSurface.position;
            position = new Vector3(position.x, position.y + 150f, position.z); //alzo livello di acqua
            waterSurface.position = position;
            rocksUnderWater.SetActive(true);
            Destroy(gameObject);
        }
    }
}
