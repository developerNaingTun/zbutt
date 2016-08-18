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
 * Description:	Colección de nodos de un árbol RTF.
 * ******************************************************************************/

using System;
using System.Collections;

namespace Net.Sgoliver.NRtfTree
{
    namespace Core
    {
        /// <summary>
        /// Colección de nodos de un documento RTF.
        /// </summary>
        public class RtfNodeCollection : CollectionBase
        {
            #region Métodos Publicos

            /// <summary>
            /// Añade un nuevo nodo a la colección actual.
            /// </summary>
            /// <param name="node">Nuevo nodo a añadir.</param>
            /// <returns>Posición en la que se ha insertado el nuevo nodo.</returns>
            public int Add(RtfTreeNode node)
            {
                InnerList.Add(node);

                return (InnerList.Count - 1);
            }

            /// <summary>
            /// Indizador de la clase RtfNodeCollection. 
            /// Devuelve el nodo que ocupa la posición 'index' dentro de la colección.
            /// </summary>
            public RtfTreeNode this[int index]
            {
                get
                {
                    return (RtfTreeNode)InnerList[index];
                }
                set
                {
                    InnerList[index] = value;
                }
            }

            /// <summary>
            /// Devuelve el índice del nodo pasado como parámetro dentro de la lista de nodos de la colección.
            /// </summary>
            /// <param name="node">Nodo a buscar en la colección.</param>
            /// <returns>Indice del nodo buscado. Devolverá el valor -1 en caso de no encontrarse el nodo dentro de la colección.</returns>
            public int IndexOf(RtfTreeNode node)
            {
                return InnerList.IndexOf(node);
            }

            /// <summary>
            /// Añade al final de la colección una nueva lista de nodos.
            /// </summary>
            /// <param name="collection">Nueva lista de nodos a añadir a la colección actual.</param>
            public void AddRange(RtfNodeCollection collection)
            {
                InnerList.AddRange(collection);
            }

            #endregion
        }
    }
}
