using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITweener  {
      void Play(float extraDelay = 0);

      void Stop();

      void Reset();

      float GetAnimationLength();

      Sequence GetAnimationSequence();
}
