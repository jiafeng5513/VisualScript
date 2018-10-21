﻿using System;
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
     *      2.2.找出[map]中剩下的node中,inputname带有<id>的node,把他们含有<id>的那个input直接改成<id>
     *      2.3.复制一个[submap]的副本[submap2]
     *      2.4.从[submap2]中删除没有input的节点和input都在[submap]中的节点
     *      2.5.遍历[submap2]:
     *          2.5.0.如果此时[submap2]为空,说明[submap]中读入了W节点,这时我们只需找到刚刚被排除的盲端节点,
     *                把他直接放回map中,并越过2.5.1-2.5.4;
     *          2.5.1.把所有node的input中的盲端点找到,放到[buffer]中
     *          2.5.2.如果发现一个node,他拥有不在[submap]中且不是盲端的输入,记这个node为<inputnode>
     *          2.5.3.把[buffer]中的节点加入到<inputnode>的输入中
     *          2.5.4.给<inputnode>改名为<id>,放回map中;
     *      2.6.如果发生了约减,那么对于map的这次遍历就此中断并从头开始
    * ==================================================================================================
    * 第二阶段:链接存储
    *
    * 0.为了加快交换速度,链接存储只是存储节点的名字,节点的真正信息存放在[Dictionary]中
    *
    * -----------------
    * |     Node      |
    * -----------------
    * |  string name  |
    * -----------------
    * |  Node next    |
    * -----------------
    * 1.创建一个[盲端表].初始化为空表
    * 2.遍历所有的Node
    * 3.每读取一个新的Node,判断他有没有input
    *      3.1.如果没有input.压到[盲端表]中,直接读取下一个节点;
    *      3.2.如果有input,证明这个节点会在将来被链接到某个节点的后面,压入[缓存]中;
    *          3.2.1.从[盲端表]和[缓存]中查找该节点的input,把找到的节点指向该节点;
    *          3.2.2.如果一个[缓存]中的节点指向了其他节点,则从[缓存]中删除他;
    *          3.2.3.处理完该节点所有的input,读取下一个节点;
    * 4.全部节点处理完,将[盲端表]的第一个节点标记为root节点;
    *
    *
    */
    public class ProtoTools
    {
        Dictionary<string, NodeDef> map = new Dictionary<string, NodeDef>();
        /// <summary>
        /// dump GraphDef into Json File
        /// </summary>
        /// <param name="data">GraphDef object</param>
        /// <param name="jsonpath">File Path for json</param>
        public static void DumpJson(GraphDef data, string jsonpath)
        {

        }
        /// <summary>
        /// for UnitTest
        /// </summary>
        public static void Decompile()
        {
            string InputFile = "E:/VisualStudio/VisualScript/utils/mnist/out/model/saved_model.pb";
            using (Stream s = new FileStream(InputFile, FileMode.Open))
            {
                GraphDef st = GraphDef.Parser.ParseFrom(s);
                for (int i = 0; i < st.Node.Count; i++)
                {
                    Console.WriteLine(st.Node[i].Name);
                }
                Console.WriteLine("全部节点名输出结束,下面是reshpe的input");
                for (int i = 0; i < st.Node[2].Input.Count; i++)
                {
                    Console.WriteLine(st.Node[2].Input[i]);
                }
            }
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
        ///   1.若[map] 中还有下一个节点node, 访问它:
        ///   2.如果node的name总含有"/"则取出第一个/之前的字串,记为<id>;
        ///     2.1.找出[map] 中所有name带有<id>的node, 从[map] 中删除他们, 并放到[submap] 中
        ///     2.2.找出[map] 中剩下的node中, inputname带有<id> 的node, 把他们含有<id> 的那个input直接改成<id>
        ///     2.3.复制一个[submap] 的副本[submap2]
        ///     2.4.从[submap2] 中删除没有input的节点和input都在[submap]中的节点
        ///     2.5.遍历[submap2]:
        ///         2.5.0.如果此时[submap2] 为空, 说明[submap] 中读入了W节点, 这时我们只需找到刚刚被排除的盲端节点,
        ///               把他直接放回map中, 并越过2.5.1-2.5.4;
        ///         2.5.1.把所有node的input中的盲端点找到,放到[buffer] 中
        ///         2.5.2.如果发现一个node,他拥有不在[submap] 中且不是盲端的输入, 记这个node为<inputnode>
        ///         2.5.3.把[buffer] 中的节点加入到<inputnode>的输入中
        ///         2.5.4.给<inputnode> 改名为<id>, 放回map中;
        ///         2.6.如果发生了约减,那么对于map的这次遍历就此中断并从头开始
        public void MapCut()
        {
            var _map = this.map;
            bool flag = true;
            while (flag)
            {
                foreach (var node in _map)
                {
                    if (node.Key.Contains("/"))
                    {
                        string id = node.Key.Split('/')[0];
                        Dictionary<string, NodeDef> submap = new Dictionary<string, NodeDef>();
                        foreach (var t_node in _map)
                        {
                            if (t_node.Key.Contains(id))
                            {
                                submap.Add(t_node.Key, t_node.Value);
                                _map.Remove(t_node.Key);
                            }
                        }

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

                        var submap2 = submap;
                        foreach (var s_node in submap2)
                        {
                            if (s_node.Value.Input.Count==0||isClosed(s_node.Value,submap))
                            {
                                submap2.Remove(s_node.Key);
                            }
                        }

                    }
                }
            }
            foreach (var node in _map)
            {
                if (node.Key.Contains("/"))
                {
                    string id = node.Key.Split('/')[0];
                    Dictionary<string, NodeDef> submap=new Dictionary<string, NodeDef>();
                    foreach (var t_node in _map)
                    {
                        if (t_node.Key.Contains(id))
                        {
                            submap.Add(t_node.Key, t_node.Value);
                            _map.Remove(t_node.Key);
                        }
                    }

                    var CanNotCut = submap;
                    foreach (var p_node in submap)
                    {
                        if (p_node.Value.Input.Count==0/*或者p节点的所有输入都在submap之内*/)
                        {
                            //从CanNotBeCut中删除这个节点的Key
                            CanNotCut.Remove(p_node.Key);
                        }
                    }
                    //此时CanNotCut中只剩下不可约减的节点
                    KeyValuePair<string,NodeDef> inputNode=new KeyValuePair<string, NodeDef>();
                    foreach (var c_node in CanNotCut)
                    {
                        if (true/*c_node的输入中存在非盲端节点*/)
                        {
                            //标记为输入节点
                            inputNode = c_node;
                        }
                        else
                        {
                            //把当前c_node的input加入到输入节点的输入中
                            //这个操作相当于越过了盲端节点和子图之间的中介,
                            //由于最终所有的子图节点都要规约到输入节点上,
                            //这里提前规约并没有问题
                        }
                    }
                }
                else
                {
                    continue;
                }
                Console.WriteLine(node.Key + node.Value);
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
                var tttt = st.Node;
                for (int i = 0; i < st.Node.Count; i++)
                {
                    map.Add(st.Node[i].Name, st.Node[i]);
                }
                

            }
        }
    }
}
