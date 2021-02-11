using System;

namespace Game.Ships
{
    public class OnProjectileDestroyedArgs : EventArgs
    {
        public string tagOfObjectHit;
    }
}