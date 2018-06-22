import numpy as np
from mpl_toolkits.mplot3d import Axes3D
import matplotlib.pyplot as plt


path = r"C:\Users\Toshiba\Documents\Vincent MAIRE\MPU\Mag_view\datas.txt"
datas = np.loadtxt(path, dtype='d', delimiter=' ')

n = len(datas)

raw = []
cal = []
for i in range(n):
    if datas[i,0] == 1:
        raw.append(i)
    else:
        cal.append(i)

datas_non_cal = datas[raw,1:]
datas_cal = datas[cal,1:]

"""
datas_non_cal = np.array([])
for i in range(n):
    if datas[i,0] == 1 and datas[i,1] == 2 and datas[i,2] == 3:
        break

if i < n:
    datas_non_cal = datas[0:i]
    datas_cal = datas[i+1:n]
else:
    exit(1)
"""

# Plots
min_bound = -600
max_bound = 600
sphere_size = 500

fig = plt.figure()
plt.subplot(221)
plt.plot(datas_non_cal[:,0],datas_non_cal[:,1],'.r',ms=1)
plt.plot(datas_cal[:,0],datas_cal[:,1],'.b',ms=1)
plt.xlabel("X")
plt.ylabel("Y")
theta = np.linspace(-np.pi, np.pi, 200)
plt.plot(sphere_size*np.sin(theta), sphere_size*np.cos(theta),c='k')
plt.gca().set_aspect('equal', adjustable='box')
plt.axis([min_bound,max_bound,min_bound,max_bound])

plt.subplot(222)
plt.plot(datas_non_cal[:,1],datas_non_cal[:,2],'.r',ms=1)
plt.plot(datas_cal[:,1],datas_cal[:,2],'.b',ms=1)
plt.xlabel("Y")
plt.ylabel("Z")
theta = np.linspace(-np.pi, np.pi, 200)
plt.plot(sphere_size*np.sin(theta), sphere_size*np.cos(theta),c='k')
plt.gca().set_aspect('equal', adjustable='box')
plt.axis([min_bound,max_bound,min_bound,max_bound])

plt.subplot(223)
plt.plot(datas_non_cal[:,2],datas_non_cal[:,0],'.r',ms=1)
plt.plot(datas_cal[:,2],datas_cal[:,0],'.b',ms=1)
plt.xlabel("Z")
plt.ylabel("X")
theta = np.linspace(-np.pi, np.pi, 200)
plt.plot(sphere_size*np.sin(theta), sphere_size*np.cos(theta),c='k')
plt.gca().set_aspect('equal', adjustable='box')
plt.axis([min_bound,max_bound,min_bound,max_bound])

fig = plt.figure()
ax = fig.add_subplot(111, projection='3d')
ax.scatter(datas_non_cal[:,0],datas_non_cal[:,1],datas_non_cal[:,2],c='r')
ax.scatter(datas_cal[:,0],datas_cal[:,1],datas_cal[:,2],c='b')
ax.set_xlim(min_bound,max_bound)
ax.set_ylim(min_bound,max_bound)
ax.set_zlim(min_bound,max_bound)

# Draw sphere
u, v = np.mgrid[0:2*np.pi:20j, 0:np.pi:10j]
x = sphere_size*np.cos(u)*np.sin(v)
y = sphere_size*np.sin(u)*np.sin(v)
z = sphere_size*np.cos(v)
ax.plot_wireframe(x, y, z, color="k", alpha=0.6, linewidth=1)
ax.set_aspect('equal')
    
plt.show()


