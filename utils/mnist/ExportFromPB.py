"""
从pb文件中导出数据
"""
import tensorflow as tf
from PIL import Image
from tensorflow.python.platform import gfile
from plot_weight import plot_conv_weights,plot_conv_output

class Predict(object):

    def __init__(self):
        # 清除默认图的堆栈，并设置全局图为默认图
        # 若不进行清楚则在第二次加载的时候报错，因为相当于重新加载了两次
        tf.reset_default_graph()
        gpu_options = tf.GPUOptions(per_process_gpu_memory_fraction=0.5)
        self.sess = tf.Session(config=tf.ConfigProto(gpu_options=gpu_options))
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

    def predict2(self, image_path):
        # 读取图片
        img = Image.open(image_path).convert('L')
        tv = list(img.getdata())
        tva = [(255 - x) * 1.0 / 255.0 for x in tv]
        x1 = [tva]

        out = self.sess.graph.get_tensor_by_name('out:0')
        input_x = self.sess.graph.get_tensor_by_name('input:0')
        Keep_prob = self.sess.graph.get_tensor_by_name('Keep_prob:0')

        w_conv1 = self.sess.graph.get_tensor_by_name('w1:0')
        tf.add_to_collection('conv1_weights', w_conv1)

        h_conv1 = self.sess.graph.get_tensor_by_name('activation1:0')
        tf.add_to_collection('conv1_output', h_conv1)

        prediction = tf.argmax(out, 1)
        predint = prediction.eval(feed_dict={input_x: x1, Keep_prob: 1.0}, session=self.sess)

        print(image_path)
        print(' Predict digit', predint[0])

        conv_weights = self.sess.run([tf.get_collection('conv1_weights')])
        for i, c in enumerate(conv_weights[0]):
            plot_conv_weights(c, 'conv{}'.format(i),'./out/plot')

        conv_out = self.sess.run([tf.get_collection('conv1_output')], feed_dict={input_x: x1})
        for i, c in enumerate(conv_out[0]):
            plot_conv_output(c, 'conv{}'.format(i),'./out/plot')

if __name__ == '__main__':
    model = Predict()
    #model.predict2('./raw/2.png')
    # model.predict2('./raw/4.png')
    model.predict2('./raw/8.png')

