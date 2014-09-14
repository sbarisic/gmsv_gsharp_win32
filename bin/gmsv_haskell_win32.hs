module Main where
import Foreign.C.Types

main = putStrLn "Hello World!"

fibonacci :: Int -> Int
fibonacci n = fibs !! n
    where fibs = 0 : 1 : zipWith (+) fibs (tail fibs)
 
gmod13_open :: CInt -> CInt
gmod13_open = fromIntegral . fibonacci . fromIntegral
 
foreign export ccall gmod13_open :: CInt -> CInt