using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using org.tensorflow.framework;

namespace ProtobufTools
{
    //借助Dictionary,实现链接存储
    /*==================================================================================================
    * 第一阶段:图约减
     * 1.若[map]中还有下一个节点node,访问它:
     * 2.如果node的name总含有"/"则取出第一个/之前的字串,记为<id>;
     *      2.1.找出[map]中所有name带有<id>的node,从[map]中删除他们,并放到[submap]中
     *          但是,如果在当前的
     *      2.2.找出[map]中剩下的node中,inputname带有<id>的node,把他们含有<id>的那个input直接改成<id>
     *      2.3.复制一个[submap]的副本[submap2]
     *      2.4.从[submap2]中删除没有input的节点和input都在[submap]中的节点
     *      2.5.遍历[submap2]:
     *          2.5.0.如果此时[submap2]为空,说明[submap]中读入了W节点,这时我们只需找到刚刚被排除的盲端节点,
     *                把他直接放回map中,并越过2.5.1-2.5.4;
     *          2.5.1.把所有node的input中的盲端点找到,放到[buffer]中
     *          2.5.2.如果发现一个node,他拥有不在[submap]中且不是盲端的输入,记这个node为<inode>
     *          2.5.3.把[buffer]中的节点加入到<inode>的输入中
     *          2.5.4.给<inode>改名为<id>,放回map中;
     *      2.6.如果发生了约减,那么对于map的这次遍历就此中断并从头开始
    * ==================================================================================================
    * 第二阶段:链接存储
     * 1.创建一个[盲端表].初始化为空表
     * 2.Node存储在[map]中
     * 3.map遍历完毕?是:转4,否:去当前项,标记为node,转3.1:
     *  3.1.判断node有没有输入,如果没有,转3.2,如果有,转3.3;
     *  3.2.把node压入[盲端表],返回3;
     *  3.3.把node压入[缓存],转入3.3.1:
     *      3.3.1.如果已经处理过node的所有input,则转3,否则,去当前输入项,记作input,转3.3.2;
     *      3.3.2.如果从[盲端表]和[缓存]中查找input,把找到的节点指向node,
     *      3.3.3.如果一个[缓存]中的节点指向了其他节点,则从[缓存]中删除他;
     * 4.将[盲端表]的第一个节点标记为root节点,清空缓存,此时从root节点依次访问能找到主线上的全部节点
     *   而除了root外,盲端表中的其他节点就是网络的常量输入
     *
    */
    public class simpleNode<T>{      
        public T value;              
        public simpleNode<T> next;

        public simpleNode(T value, simpleNode<T> next=null)
        {
            this.value = value;
            this.next = next;
        }
    }
    public class ProtoTools
    {
        Dictionary<string, NodeDef> map = new Dictionary<string, NodeDef>();
        public Dictionary<string, NodeDef> Map { get => map; set => map = value; }
        private string rootNodeName = "";
        public LinkedList<NodeDef> SimpleMap=new LinkedList<NodeDef>();

        /// <summary>
        /// dump GraphDef into Json File
        /// </summary>
        /// <param name="data">GraphDef object</param>
        /// <param name="jsonpath">File Path for json</param>
        public static void DumpJson(GraphDef data, string jsonpath)
        {

        }

        /// <summary>
        /// 判断一个节点的输入是否在给定的集合范围内取值
        /// </summary>
        /// <param name="node"></param>
        /// <param name="set"></param>
        /// <returns></returns>
        private bool isClosed(NodeDef node, Dictionary<string, NodeDef>set)
        {
            for (int i = 0; i < node.Input.Count; i++)
            {
                if (!set.ContainsKey(node.Input[i]))
                {
                    return false;
                }
            }
            return true;
        }
        /// <summary>
        /// 图约减
        /// </summary>
        public void MapCut()
        {
            var _map = this.map;
            bool flag = true;
            while (flag)
            {
                flag = false;
                foreach (var node in _map)
                {
                    //如果node的name总含有"/"则取出第一个/之前的字串,记为<id>;
                    if (node.Key.Contains("/"))
                    {
                        string id = node.Key.Split('/')[0];
                        if (id == "reshape")
                        {
                            //reshape节点不需要规约(这是一个瑕疵)
                            continue;
                        }
                        Dictionary<string, NodeDef> submap = new Dictionary<string, NodeDef>();
                        //找出[_map] 中所有name带有<id>的node, 从[_map] 中删除他们, 并放到[submap] 中
                        foreach (var t_node in _map)
                        {
                            if (t_node.Key.Contains(id))
                            {
                                submap.Add(t_node.Key, t_node.Value);  
                            }
                        }
                        foreach (var d_node in submap)
                        {
                            _map.Remove(d_node.Key);
                        }
                        //找出[_map] 中剩下的node中, inputname带有<id> 的node, 把他们含有< id > 的那个input直接改成 < id >
                        foreach (var r_node in _map)
                        {
                            for (int i = 0; i < r_node.Value.Input.Count; i++)
                            {
                                if (r_node.Value.Input[i].Contains(id))
                                {
                                    r_node.Value.Input[i] = id;
                                }
                            }
                        }
                        //复制一个[submap] 的副本[submap2]
                        var submap2 = submap;
                        //从[submap2] 中删除没有input的节点和input都在[submap]中的节点
                        var recycleBin =new Dictionary<string, NodeDef>();//回收站
                        foreach (var s_node in submap2)
                        {
                            if (s_node.Value.Input.Count==0||isClosed(s_node.Value,submap))
                            {
                                recycleBin.Add(s_node.Key, s_node.Value);
                            }
                        }

                        foreach (var r_node in recycleBin)
                        {
                            submap2.Remove(r_node.Key);
                        }
                        //如果此时[submap2] 为空, 说明[submap] 中读入了W节点, 这时我们只需找到刚刚被排除的盲端节点,把他直接放回map中
                        if (submap2.Count==0)
                        {
                            //这种情况是由于约减到了W项导致的,基于不重名假设,我们直接把权重项拉回来
                            foreach (var r_node in recycleBin)
                            {
                                if (r_node.Value.Input.Count==0)
                                {
                                    _map.Add(r_node.Key, r_node.Value);
                                    break;
                                }
                            }
                        }
                        else
                        {
                            //把所有s_node的input中的盲端点找到,放到[buffer] 中
                            var buffer= new Dictionary<string, NodeDef>();
                            var iNode= new NodeDef();
                            foreach (var s_node in submap2)
                            {
                                for (int i = 0; i < s_node.Value.Input.Count; i++)
                                {
                                    var inputNodeOfs_node = new NodeDef();
                                    if (submap.TryGetValue(s_node.Value.Input[i], out inputNodeOfs_node))
                                    {
                                        if (inputNodeOfs_node.Input.Count == 0)
                                        {
                                            buffer.Add(inputNodeOfs_node.Name, inputNodeOfs_node);
                                        }
                                    }
                                    else
                                    {
                                        //如果发现一个node,他拥有不在[submap] 中且不是盲端的输入, 记这个node为<inode>
                                        //注意,由于这个节点在[submap]中找不到,所以要扩大寻找范围
                                        if (map.TryGetValue(s_node.Value.Input[i], out inputNodeOfs_node)
                                            && (inputNodeOfs_node.Input.Count != 0|| inputNodeOfs_node.Name==rootNodeName))
                                        {
                                            //发现iNode
                                            iNode = s_node.Value;
                                        }
                                    }
                                }
                            }
                            //把[buffer] 中的节点加入到<inode>的输入中
                            foreach (var b_node in buffer)
                            {
                                iNode.Input.Add(b_node.Value.Name);
                            }
                            //给<inputnode> 改名为<id>, 放回_map中
                            iNode.Name = id;
                            _map.Add(iNode.Name,iNode);
                        }
                        //如果发生了约减,那么对于_map的这次遍历就此中断并从头开始
                        flag = true;
                        break;
                    }
                }//end of foreach (var node in _map)
            }//end of while (flag)
            //到这里已经完成约减
            map = _map;
        }
        /// <summary>
        /// 创建链接存储
        /// </summary>
        /// 1.创建一个[盲端表].初始化为空表
        /// 2.Node存储在[map] 中
        /// 3.map遍历完毕? 是:转4,否:去当前项,标记为node,转3.1:
        ///  3.1.判断node有没有输入,如果没有,转3.2,如果有,转3.3;
        ///  3.2.把node压入[盲端表],返回3;
        ///  3.3.把node压入[缓存],转入3.3.1:
        ///      3.3.1.如果已经处理过node的所有input,则转3,否则,去当前输入项,记作input,转3.3.2;
        ///      3.3.2.如果从[盲端表] 和[缓存]中查找input,把找到的节点指向node,
        ///      3.3.3.如果一个[缓存] 中的节点指向了其他节点, 则从[缓存]中删除他;
        /// 4.将[盲端表] 的第一个节点标记为root节点, 清空缓存, 此时从root节点依次访问能找到主线上的全部节点
        ///   而除了root外,盲端表中的其他节点就是网络的常量输入
        public void BuildLinkedList()
        {
            var rootNode = new NodeDef();
            var BlindEndList = new Dictionary<string, simpleNode<NodeDef>>();
            var Buffer= new Dictionary<string, simpleNode<NodeDef>>();

            foreach (var node in map)
            {
                if (node.Value.Input.Count==0)
                {
                    BlindEndList.Add(node.Key,new simpleNode<NodeDef>(node.Value));
                }
                else
                {
                    Buffer.Add(node.Key, new simpleNode<NodeDef>(node.Value));
                    for (int i = 0; i < node.Value.Input.Count; i++)
                    {
                        if (BlindEndList.ContainsKey(node.Value.Input[i]))
                        {
                            BlindEndList[node.Value.Input[i]].next = Buffer[node.Key];
                        }
                        else if (Buffer.ContainsKey(node.Value.Input[i]))
                        {
                            Buffer[node.Value.Input[i]].next = Buffer[node.Key];
                            Buffer.Remove(node.Value.Input[i]);
                        }
                    }
                }                
            }

            
            var tempLList_head = BlindEndList[rootNodeName];
            var tempLList_current= tempLList_head.next;

            SimpleMap.AddFirst(BlindEndList[rootNodeName].value);
            LinkedListNode<NodeDef> SimpleMap_current = SimpleMap.First;

            while (tempLList_current!=null)
            {
                SimpleMap.AddAfter(SimpleMap_current, tempLList_current.value);
                tempLList_current = tempLList_current.next;
                SimpleMap_current = SimpleMap_current.Next;
            }


        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="InputFile"></param>
        public void Decompile(string InputFile)
        {
            using (Stream s = new FileStream(InputFile, FileMode.Open))
            {
                GraphDef st = GraphDef.Parser.ParseFrom(s);
                //遍历所有节点,转储到一个Dictionary中
                for (int i = 0; i < st.Node.Count; i++)
                {
                    map.Add(st.Node[i].Name, st.Node[i]);
                }

                rootNodeName = st.Node[0].Name;//标记起点

            }
        }
    }
}
