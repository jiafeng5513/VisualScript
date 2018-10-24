# -*- coding: utf-8 -*-
"""
Created on Tue Jun 12 09:36:55 2018
func：加载模型，进行模型测试
@author: kuangyongjian
"""
import tensorflow as tf
import numpy as np
from PIL import Image
from tensorflow.python.platform import gfile
import matplotlib.pyplot as plt



class Predict(object):

    def __init__(self):
        # 清除默认图的堆栈，并设置全局图为默认图
        # 若不进行清楚则在第二次加载的时候报错，因为相当于重新加载了两次
        tf.reset_default_graph()
        self.sess = tf.Session()
        self.sess.run(tf.global_variables_initializer())

        # 加载模型到sess中
        self.restore()
        print('load susess')

    def restore(self):
        # self.restore_mode_pb('E:/VisualStudio/VisualScript/utils/mnist/out_without_dropout/model/saved_model_without_dropout.pb')
        self.restore_mode_pb('E:/VisualStudio/VisualScript/utils/mnist/out/model/saved_model.pb')

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
        flatten_img = np.reshape(img, 784)
        image = flatten_img.astype(np.float32)
        images = np.multiply(image, 1.0 / 255.0)
        x = np.array([1 - images])

        y_ =self.sess.graph.get_tensor_by_name('out:0')
        input_x = self.sess.graph.get_tensor_by_name('input:0')
        Keep_prob = self.sess.graph.get_tensor_by_name('Keep_prob:0')
        y = self.sess.run(y_, feed_dict={input_x: x, Keep_prob: 1.0})

        # print("y_",y_[0],y_[1],y_[2],y_[3],y_[4],y_[5],y_[6],y_[7],y_[8],y_[9])
        #print("y",y)
        print(image_path)
        print(' Predict digit', np.argmax(y[0]))

    def predict2(self, image_path):
        # 读取图片
        img = Image.open(image_path).convert('L')
        tv = list(img.getdata())
        tva = [(255 - x) * 1.0 / 255.0 for x in tv]
        flatten_img = np.reshape(img, 784)
        image = flatten_img.astype(np.float32)
        images = np.multiply(image, 1.0 / 255.0)
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
    model.predict2('E:/VisualStudio/VisualScript/utils/mnist/input_data/raw/mnist_train_12.jpg')# 1
    model.predict2('E:/VisualStudio/VisualScript/utils/mnist/input_data/raw/mnist_train_11.jpg')# 3
    model.predict2('E:/VisualStudio/VisualScript/utils/mnist/input_data/raw/mnist_train_15.jpg')# 0
    model.predict2('E:/VisualStudio/VisualScript/utils/mnist/input_data/raw/mnist_train_8.jpg') # 9
    model.predict2('E:/VisualStudio/VisualScript/utils/mnist/input_data/raw/mnist_train_9.jpg') # 8
    model.predict2('E:/VisualStudio/VisualScript/utils/mnist/input_data/raw/mnist_train_10.jpg')# 0
    model.predict2('E:/VisualStudio/VisualScript/utils/mnist/input_data/raw/mnist_train_18.jpg')  # 6