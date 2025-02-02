using System;

namespace Entity
{
    public enum Element
    {
        Physical,
        Gun,
        Fire,
        Ice,
        Electric,
        Force,
        Nerve,
        Expel,
        Curse,
        Almighty,
        Recovery,
        Support,
        Other
    }

    [Serializable]
    public struct ElementResistances
    {
        public ResistanceType physical;
        public ResistanceType gun;
        public ResistanceType fire;
        public ResistanceType ice;
        public ResistanceType electricity;
        public ResistanceType force;
        public ResistanceType nerve;
        public ResistanceType expel;
        public ResistanceType curse;
        public ResistanceType almighty;
    }
}