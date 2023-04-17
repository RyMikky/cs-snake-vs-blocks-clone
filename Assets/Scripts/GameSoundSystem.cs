using UnityEngine;

public class GameSoundSystem : MonoBehaviour
{
    public AudioSource _snakeEatOne;
    public AudioSource _snakeEatTwo;
    public AudioSource _boxBreak;
    public AudioSource _buttonClickSource;
    public AudioSource _backgroundSource;

    private System.Random _random = new System.Random();

    public void PlayClick()
    {
        _buttonClickSource.Play();
    }

    public void PlaySnakeEatOne()
    {
        _snakeEatOne.Play();
    }

    public void PlaySnakeEatTwo()
    {
        _snakeEatTwo.Play();
    }

    public void PlaySnakeEat()
    {
        if (_random.Next(0, 2) > 0)
        {
            PlaySnakeEatOne();
        }
        else
        {
            PlaySnakeEatTwo();
        }
    }

    public void PlayBoxBreak()
    {
        _boxBreak.Play();
    }

    public void IncrementBoxBreakPitch()
    {
        if (_boxBreak.pitch < 3) _boxBreak.pitch += 0.1f;
    }

    public void SetBoxBreakPitchDefault()
    {
        _boxBreak.pitch = 1;
    }

    public void SetBackgroundVolume(float volume)
    {
        _backgroundSource.volume = volume;
    }

    public float BackgroundVolume()
    {
        return _backgroundSource.volume;
    }

    public void PLayBackground()
    {
        _backgroundSource.loop = true;
        _backgroundSource.Play();
    }
}
