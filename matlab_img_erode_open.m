originalBW = imread('');

erodeBW = imerode(originalBW,[1, 1, 1; 1, 1, 1; 1, 1, 1]);

openBW = imopen(originalBW, [1,1,1;1,1,1;1,1,1]);