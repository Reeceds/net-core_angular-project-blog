import { Images } from './images';

export interface Post {
  id: number;
  displayName: string;
  title: string;
  description: string;
  dateCreated: Date;
  currentImage: string;
  images: Images[];
}
