# -*- coding: utf-8 -*-
"""
Created on Tue Jun 12 09:36:55 2018
func：加载模型，进行模型测试
@author: kuangyongjian
"""
import tensorflow as tf
from PIL import Image
from tensorflow.python.platform import gfile


class Predict(object):

    def __init__(self):
        # 清除默认图的堆栈，并设置全局图为默认图
        # 若不进行清楚则在第二次加载的时候报错，因为相当于重新加载了两次
        tf.reset_default_graph()
        self.sess = tf.Session()
        self.sess.run(tf.global_variables_initializer())

        # 加载模型到sess中
        self.restore_mode_pb('./out/model/model_minimal.pb')
        print('load susess')


    def restore_mode_pb(self,pb_file_path):
        self.sess = tf.Session()
        with gfile.FastGFile(pb_file_path, 'rb') as f:
            graph_def = tf.GraphDef()
            graph_def.ParseFromString(f.read())
            self.sess.graph.as_default()
            tf.import_graph_def(graph_def, name='')

    def predict(self, image_path):
        # 读取图片
        img = Image.open(image_path).convert('L')
        tv = list(img.getdata())
        tva = [(255 - x) * 1.0 / 255.0 for x in tv]
        x = [tva]

        out = self.sess.graph.get_tensor_by_name('out:0')
        input_x = self.sess.graph.get_tensor_by_name('input:0')
        Keep_prob = self.sess.graph.get_tensor_by_name('Keep_prob:0')

        prediction = tf.argmax(out, 1)
        predint = prediction.eval(feed_dict={input_x: x, Keep_prob: 1.0}, session=self.sess)

        print(image_path)
        print(' Predict digit', predint[0])

if __name__ == '__main__':
    model = Predict()
    model.predict('./raw/2.png')
    model.predict('./raw/4.png')
    model.predict('./raw/8.png')
