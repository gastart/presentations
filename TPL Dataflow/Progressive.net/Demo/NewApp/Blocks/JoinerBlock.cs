using System.Threading.Tasks.Dataflow;
using Common.Domain;

namespace NewApp.Blocks
{
    public class JoinerBlock
    {
        private JoinBlock<Round, Round> _block;

        public JoinerBlock()
        {
            _block = new JoinBlock<Round, Round>();

        }

        public void T()
        {
            
        }
       

    }
}