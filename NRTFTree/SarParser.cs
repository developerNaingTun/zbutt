/********************************************************************************
 *   This file is part of NRtfTree.
 *
 *   NRtfTree is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 2 of the License, or
 *   (at your option) any later version.
 *
 *   NRtfTree is distributed in the hope that it will be useful,
 *   but WITHOUT ANY WARRANTY; without even the implied warranty of
 *   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *   GNU General Public License for more details.
 *
 *   You should have received a copy of the GNU General Public License
 *   along with NRtfTree; if not, write to the Free Software
 *   Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
 ********************************************************************************/	

/********************************************************************************
 * Library:		NRtfTree
 * Version:     v0.2.1
 * Class:		ImageNode
 * Copyright:   2005 Salvador Gomez
 * Home Page:	http://www.sgoliver.net
 * SF Project:	http://nrtftree.sourceforge.net
 *				http://sourceforge.net/projects/nrtftree
 * Date:		10/12/2006
 * Description:	Procesador abstracto utilizado por la clase RtfReader.
 * ******************************************************************************/

using System;

namespace Net.Sgoliver.NRtfTree
{
    namespace Core
    {
        /// <summary>
        /// Esta clase, utilizada por RtfReader, contiene todos los m�todos necesarios para tratar cada uno de 
        /// los tipos de elementos presentes en un documento RTF. Estos m�todos ser�n llamados autom�ticamente 
        /// durante el an�lisis del documento RTF realizado por la clase RtfReader.
        /// </summary>
        public abstract class SarParser
        {
            /// <summary>
            /// Este m�todo se llama una s�la vez al comienzo del an�lisis del documento RTF.
            /// </summary>
            public abstract void StartRtfDocument();
            /// <summary>
            /// Este m�todo se llama una s�la vez al final del an�lisis del documento RTF.
            /// </summary>
            public abstract void EndRtfDocument();
            /// <summary>
            /// Este m�todo se llama cada vez que se lee una llave de comienzo de grupo RTF.
            /// </summary>
            public abstract void StartRtfGroup();
            /// <summary>
            /// Este m�todo se llama cada vez que se lee una llave de fin de grupo RTF.
            /// </summary>
            public abstract void EndRtfGroup();
            /// <summary>
            /// Este m�todo se llama cada vez que se lee una palabra clave RTF.
            /// </summary>
            /// <param name="key">Palabra clave leida del documento.</param>
            /// <param name="hasParameter">Indica si la palabra clave va acompa�ada de un par�metro.</param>
            /// <param name="parameter">
            /// Par�metro que acompa�a a la palabra clave. En caso de que la palabra clave no vaya acompa�ada
            /// de ning�n par�metro, es decir, que el campo hasParam sea 'false', 
            /// este campo contendr� el valor 0.
            /// </param>
            public abstract void RtfKeyword(string key, bool hasParameter, int parameter);
            /// <summary>
            /// Este m�todo se llama cada vez que se lee un s�mbolo de Control RTF.
            /// </summary>
            /// <param name="key">S�mbolo de Control leido del documento.</param>
            /// <param name="hasParameter">Indica si el s�mbolo de Control va acompa�ado de un par�metro.</param>
            /// <param name="parameter">
            /// Par�metro que acompa�a al s�mbolo de Control. En caso de que el s�mbolo de Control no vaya acompa�ado
            /// de ning�n par�metro, es decir, que el campo hasParam sea 'false', 
            /// este campo contendr� el valor 0.
            /// </param>
            public abstract void RtfControl(string key, bool hasParameter, int parameter);
            /// <summary>
            /// Este m�todo se llama cada vez que se lee un fragmento de Texto del documento RTF.
            /// </summary>
            /// <param name="text">Texto leido del documento.</param>
            public abstract void RtfText(string text);
        }
    }
}
