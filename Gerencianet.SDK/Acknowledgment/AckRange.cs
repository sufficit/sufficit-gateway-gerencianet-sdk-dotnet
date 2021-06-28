using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace GerencianetSDK.Acknowledgment
{
    public class AckRange : List<AckRow>
    {
        /// <summary>
        /// Preencher esta lista apartir de um arquivo .ret <br />
        /// Arquivo de confirmações do gerencianet
        /// </summary>
        /// <param name="retStream"></param>
        public async Task Fill(Stream retStream)
        {
            using (TextReader reader = new StreamReader(retStream))
            {
                string textRow;
                while ((textRow = await reader.ReadLineAsync()) != null)
                {
                    if (!string.IsNullOrWhiteSpace(textRow))                    
                        Add(textRow);                    
                }
            }
        }

    }
}
