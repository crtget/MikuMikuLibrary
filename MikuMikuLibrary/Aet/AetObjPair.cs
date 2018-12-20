using System.Collections.Generic;
using MikuMikuLibrary.Aet.Body;
using MikuMikuLibrary.IO.Common;

namespace MikuMikuLibrary.Aet
{
    /// <summary>
    /// A set of <see cref="AetObj"/>s referenced by an <see cref="AetObjPairPointer"/>.
    /// </summary>
    public class AetObjPair
    {
        /// <summary>
        /// The name given to this <see cref="AetObjPair"/> by an <see cref="EffBody"/>.
        /// </summary>
        public string PairName { get; set; }

        /// <summary>
        /// The <see cref="AetObj"/> <see cref="List{T}"/> that makes up this <see cref="AetObjPair"/>.
        /// </summary>
        public List<AetObj> AetObjects;

        public AetObjPair()
        {
            return;
        }

        public AetObjPair(List<AetObj> aetObjects)
        {
            AetObjects = aetObjects;
        }

        internal void Write(EndianBinaryWriter writer, AetSection parentAet)
        {
            foreach (var aetObj in AetObjects)
            {
                aetObj.Write(writer, parentAet);
            }
        }
    }
}