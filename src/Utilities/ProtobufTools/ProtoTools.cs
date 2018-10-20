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
    * 1.遍历所有的Node
    * 2.读取一个Node的名字,观察名字中是否含有"/",如果含有/,则把/前的部分子串取出,命名为<id>,
    *   并查找所有名字中以<id>为前缀或等于<id>的Node:
    *      2.1.如果不存在这样的节点,则直接读取下一个节点;
    *      2.2.存在这样的节点:
    *          2.2.1.把所有符合条件的节点移动到一个集合[zoo]中;
    *          2.2.2.如果集合中的node满足如下条件,标记为"可约减":
    *              ① 这个node没有input节点
    *              ② 这个node的全部input节点在[zoo]内
    *              <标记结束后,剩下的节点有两种,一种是这块子图的入口,另一种是拥有盲端输入的节点>
    *          2.2.3.查找所有未被标记为"可约减"的节点,并放入[缓存],然后遍历[缓存]
    *              2.2.3.1.查看该节点的输入:
    *                  2.2.3.1.1.如果输入中存在非盲端节点输入,则该节点标记为"入口";
    *                  2.2.3.1.2.如果输入中全部是盲端节点,则把该盲端节点加入到"入口"节点的输入中,
    *                            并将该节点标记为"可约减";
    *          2.2.4.删除所有的"可约减"节点,并把"入口"节点更名为<id>,然后把<id>加入到[id缓存]中;
    *          2.2.5.读取下一个节点,返回2;
    * 3.全部Node处理完,再进行一次遍历
    *      3.1.如果当前节点的input中出现了前缀为[id缓存]中的id的项目,则把这条input改成对应的id
    * 4.约减完成.
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
        /// 图约减
        /// </summary>
        public void MapCut()
        {
            var _map = this.map;
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

                    var CanNotCut = submap.Keys;
                    foreach (var p_node in submap)
                    {
                        if (p_node.Value.Input.Count==0/*或者p节点的所有输入都在submap之内*/)
                        {
                            //从CanNotBeCut中删除这个节点的Key

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
