﻿namespace MSE
{
    public class InteractionMatrix
    {
        int[] collisionMask;

        public InteractionMatrix(int size)
        {
            UnityEngine.Debug.Assert(size <= sizeof(int));
            collisionMask = new int[size];
        }

        public void SetInteractable(int layerIndexL, int layerIndexR)
        {
            collisionMask[layerIndexL] |= 1 << layerIndexR;
            collisionMask[layerIndexR] |= 1 << layerIndexL;
        }

        public void SetInteractableAll(int layerIndex)
        {
            for (int i = 0; i < collisionMask.Length; ++i)
            {
                SetInteractable(layerIndex, i);
            }
        }

        public void ClearInteractable(int layerIndexL, int layerIndexR)
        {
            collisionMask[layerIndexL] &= ~(1 << layerIndexR);
            collisionMask[layerIndexR] &= ~(1 << layerIndexL);
        }

        public bool TestInteractable(int layerIndexL, int layerIndexR)
        {
            return (collisionMask[layerIndexL] & (1 << layerIndexR)) != 0;
        }
    }
}
